using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance => _instance;

    UIManager        _ui;
    DataManager      _data;
    SaveManager      _save;
    SettingManager   _setting;
    LanguageManager  _language;
    AudioManager     _audio;
    CameraManager    _camera;
    InputManager     _input;

    public UIManager        UI => _ui;
    public DataManager      Data => _data;
    public SaveManager      Save => _save;
    public SettingManager   Setting => _setting;
    public LanguageManager  Language => _language;
    public AudioManager     Audio => _audio;
    public CameraManager    Camera => _camera;
    public InputManager     Input => _input;




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
        }
    }

    void InitializeManagers()
    {
        // UI를 만들어서 유저에게 보여줄 수 있는 공간 만들기
        // 데이터 불러오기
        // 유저 세이브 불러오기
        // 설정값을 찾아서 세팅
        // 언어도 세팅
        // 사운드 세팅
        // 카메라 초기화
        // 유저 입력 받기 시작
        CreatManager(ref _ui);
        CreatManager(ref _data);
        CreatManager(ref _save);
        CreatManager(ref _setting);
        CreatManager(ref _language);
        CreatManager(ref _audio);
        CreatManager(ref _camera);
        CreatManager(ref _input);
    }

    //달라지는 것이 "자료형"뿐이라면 "자료형"에 따라 변수로 작용하는 함수를 만들 수 있다
    //Generic Method : 범용 함수
    //반환값 이름<자료형>(매개변수) where 자료형 : 부모

    // 원본 값을 "참조" 한다. 원본 값이랑 연결되는 변수로 만들어준다. Reference - ref
    ManagerType CreatManager<ManagerType>(ref ManagerType targetVariable) where ManagerType : ManagerBase
    {
        if (targetVariable == null)
        {
            targetVariable = gameObject.AddComponent<ManagerType>();
            targetVariable.Connect(this);
        }
        return targetVariable;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
