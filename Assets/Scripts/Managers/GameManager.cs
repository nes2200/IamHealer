using System.Collections;
using UnityEngine;

public delegate void InitializeEvent();
public delegate void UpdateEvent(float deltaTime);
public delegate void DestroyEvent();

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance => _instance;

    UIManager        _ui;
    DataManager      _data;
    ObjectManager    _objectM;
    SaveManager      _save;
    SettingManager   _setting;
    LanguageManager  _language;
    AudioManager     _audio;
    CameraManager    _camera;
    InputManager     _input;

    public UIManager        UI => _ui;
    public DataManager      Data => _data;
    public ObjectManager    ObjectM => _objectM;
    public SaveManager      Save => _save;
    public SettingManager   Setting => _setting;
    public LanguageManager  Language => _language;
    public AudioManager     Audio => _audio;
    public CameraManager    Camera => _camera;
    public InputManager     Input => _input;

    IEnumerator initializing;

    public static event InitializeEvent OnInitializeManager;
    public static event InitializeEvent OnInitializeController;
    public static event InitializeEvent OnInitializeCharacter;
    public static event InitializeEvent OnInitializeObject;
    public static event UpdateEvent     OnUpdateManager;
    public static event UpdateEvent     OnUpdateController;
    public static event UpdateEvent     OnUpdateCharacter;
    public static event UpdateEvent     OnUpdateObject;
    public static event DestroyEvent    OnDestroyManager;
    public static event DestroyEvent    OnDestroyController;
    public static event DestroyEvent    OnDestroyCharacter;
    public static event DestroyEvent    OnDestroyObject;

    bool isLoading = true;
    bool isPlaying = true;

    //Awake     : 시작할 때 (아침에 눈을 뜸)
    //OnEnabled : 시작할 때 (정신 차림)
    //OnDisabled: 기절
    //Start     : 시작할 때 (하루의 시작)
    void Awake()
    {
        //세상에 단 하나만 있도록 유지하는 패턴 : 싱글턴 패턴(Singleturn Pattern)S
        if(Instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        // IEnumerator => 반복자. 반복해서 함수가 실행됨 => 프레임 단위로 기다렸다가 실행.
        // 한 번 실행하고 Yield 양보 했다가 다음 프레임에 또 나와서 실행하고 반복
        // Ienumerator는 쓴다고 실행되는게 아니라 StartCoroutine()을 써야 진행된다.
        initializing = InitializeManagers();
        StartCoroutine(initializing);
    }

    private void OnDestroy() //매니저가 없어지면
    {
        if(initializing != null) StopCoroutine(initializing);
        DeleteManagers();
    }

    IEnumerator InitializeManagers()
    {
        // UI를 만들어서 유저에게 보여줄 수 있는 공간 만들기
        // 데이터 불러오기
        // 유저 세이브 불러오기
        // 설정값을 찾아서 세팅
        // 언어도 세팅
        // 사운드 세팅
        // 카메라 초기화
        // 유저 입력 받기 시작
        int totalLoadCount = 0;
        totalLoadCount += CreatManager(ref _ui).LoadCount;
        totalLoadCount += CreatManager(ref _data).LoadCount;
        totalLoadCount += CreatManager(ref _objectM).LoadCount;
        totalLoadCount += CreatManager(ref _save).LoadCount;
        totalLoadCount += CreatManager(ref _setting).LoadCount;
        totalLoadCount += CreatManager(ref _language).LoadCount;
        totalLoadCount += CreatManager(ref _audio).LoadCount;
        totalLoadCount += CreatManager(ref _camera).LoadCount;
        totalLoadCount += CreatManager(ref _input).LoadCount;

        yield return UI.Initialize(this); 
        UIBase loadingUI =  UIManager.ClaimOpenScreen(UIType.Loading); //UI가 연결됬으니 기능 실행해보기
        IProgress<int> loadingProgress = loadingUI as IProgress<int>;
        loadingProgress?.Set(0, totalLoadCount);

        yield return Data.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return ObjectM.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return UI.Connect(this);


        yield return Save.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Setting.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Language.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Audio.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Camera.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Input.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return null;

        Pause();

        InputManager.OnAnyKey -= UnPause;
        InputManager.OnAnyKey += UnPause;

        loadingProgress.SetComplete();

        yield return new WaitUntil(() => isPlaying);
        UIManager.ClaimOpenScreen(UIType.Title, ScreenChangeType.ScreenChanger);
        InputManager.OnAnyKey -= UnPause;

        isLoading = false;
    }

    void DeleteManagers()
    {
        // 유저 입력
        Input?.Disconnect();
        // 오브젝트 제거
        ObjectM.Disconnect();
        // 사운드
        Audio?.Disconnect();
        // 언어 
        Language?.Disconnect();
        // 설정
        Setting?.Disconnect();
        // 유저 세이브
        Save?.Disconnect();
        // 카메라 
        Camera?.Disconnect();
        // UI
        UI?.Disconnect();
        // 데이터
        Data?.Disconnect();
    }

    //달라지는 것이 "자료형"뿐이라면 "자료형"에 따라 변수로 작용하는 함수를 만들 수 있다
    //Generic Method : 범용 함수
    //반환값 이름<자료형>(매개변수) where 자료형 : 부모

    // 원본 값을 "참조" 한다. 원본 값이랑 연결되는 변수로 만들어준다. Reference - ref
    ManagerType CreatManager<ManagerType>(ref ManagerType targetVariable) where ManagerType : ManagerBase
    {
        if (targetVariable == null)
        {
            targetVariable = this.TryAddComponent<ManagerType>();
        }
        return targetVariable;
    }

    public static void QuitGame()
    {
        //게임을 끈다는 것은? -> 윈도우에게 해당 프로그램을 실행 목록에서 제거해달라고 하기
        //프로그램이라고 부르는 것은 실제로 윈도우가 뭐라고 알고 있을까? -> exe -> Executable application
        //그냥 끄면? -> 유니티 에디터가 꺼진다!
        //에디터에서 할 일과 실제 빌드에서 해야하는 일이 다르다
        //그러면 코드를 실제로 컴파일 하기 전에 구분을 해줄 수 있어야 함
        //미리 처리해놓기 -> 전처리기 -> #으로 시작하는 코드
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void Pause()
    {
        Instance.isPlaying = false;
    }
    public static void UnPause()
    {
        Instance.isPlaying = true;
    }



    void InvokeInitializeEvent(ref InitializeEvent originEvent)
    {
        if(originEvent != null)
        {
        InitializeEvent currentEvent = originEvent;
        originEvent = null;
        currentEvent.Invoke();
        }
    }
    void InvokeDestroyEvent(ref DestroyEvent originEvent)
    {
        if (originEvent != null)
        {
            DestroyEvent currentEvent = originEvent;
            originEvent = null;
            currentEvent.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //게임 진행을 할 수 있는지 여부를 조정할 수 있어야함
        //초기화 해야하는지, 하지 말아야 하는지
        //pause 상태 => 업데이트 하지 않는다
        if (isLoading) return;

        //매니저를 초기화
        InvokeInitializeEvent(ref OnInitializeManager);
        //캐릭터를 초기화
        InvokeInitializeEvent(ref OnInitializeCharacter);
        //컨트롤러를 초기화
        InvokeInitializeEvent(ref OnInitializeController);
        //오브젝트를 초기화
        InvokeInitializeEvent(ref OnInitializeObject);


        if (isPlaying)
        {
            float deltaTime = Time.deltaTime;
            //매니저를 업데이트
            OnUpdateManager?.Invoke(deltaTime);
            //컨트롤러를 업데이트
            OnUpdateController?.Invoke(deltaTime);
            //캐릭터를 업데이트
            OnUpdateCharacter?.Invoke(deltaTime);
            //오브젝트를 업데이트
            OnUpdateObject?.Invoke(deltaTime);
        }

        //오브젝트를 제거
        InvokeDestroyEvent(ref OnDestroyObject);
        //컨트롤러를 제거
        InvokeDestroyEvent(ref OnDestroyController);
        //캐릭터를 제거
        InvokeDestroyEvent(ref OnDestroyCharacter);
        //매니저를 제거
        InvokeDestroyEvent(ref OnDestroyManager);
    }
}
