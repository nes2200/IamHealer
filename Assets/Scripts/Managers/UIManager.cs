using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UIType
{
    None, Loading, Title, Option, Movable, Menu, Info, SaveSlot, Stage, GameQuit, Sandbox, BattleResult,
    _Length
}

public enum ScreenChangeType
{
    None, ScreenChanger, SlideChanger,
    _Length
}

//팝업이 일어나는 이벤트가 발생할 것이다
//델리게이트 => 스킬을 무한히 배울 수 있음
//A스킬과 B스킬을 가르쳐 놨다 => 동시에 실행시키면 => 맨 마지막 결과만 알려준다
public delegate void PopUpEvent(string title, string context, string confirm);

public class UIManager : ManagerBase
{
    public static event PopUpEvent OnPopUp;

    readonly KeyValuePair<UIType, string>[] globalScreenArray =
    {
        new(UIType.Title, "TitleScreen"),
        new(UIType.Option, "OptionScreen"),
        new(UIType.SaveSlot, "SaveLoadScreen"),
        new(UIType.Sandbox, "SandboxScreen"),
        new(UIType.Stage, "StageScreen")
    };

    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    UIBase _movableScreen;
    RectTransform switcherTransform;
    RectTransform createdTransfrom;
    RectTransform changerTransform;

    GraphicRaycaster _raycaster;
    public GraphicRaycaster Raycaster => _raycaster;

    Dictionary<UIType, UIBase> uiDictionary = new();
    Dictionary<ScreenChangeType, UI_ScreenChanger> screenChangerDictionary = new();

    Rect _uiBoundary;
    public static Rect UIBoundary => GameManager.Instance?.UI?._uiBoundary ?? Rect.zero;

    UIType _currentScreenType;
    public static UIType CurrentScreen => GameManager.Instance?.UI?._currentScreenType ?? UIType.None;

    UI_ScreenChanger currentScreenChanger;

    float _uiScale = 1.0f;
    public static float UIScale => GameManager.Instance?.UI?._uiScale ?? 1.0f;

    public IEnumerator Initialize(GameManager newManager)
    {
        //GameObject.FindGameObjectsWithTag("MainCavas");
        yield return null;
        SetMainCanvas(GetComponentInChildren<Canvas>());

        SetUI(UIType.Loading, GetComponentInChildren<UI_LoadingScreen>());
        yield return null;
    }

    public RectTransform CreateFullScreen(string wantName)
    {
        GameObject instance = new GameObject(wantName);
        RectTransform result = instance.AddComponent<RectTransform>();
        result.SetParent(MainCanvas.transform);
        //맨 위로 올리기
        result.SetAsFirstSibling();
        //anchor를 stretch - stretch로 만들고
        result.anchorMin = Vector3.zero;
        result.anchorMax = Vector3.one;
        //여백을 0,0,0,0
        result.offsetMin = Vector3.zero;
        result.offsetMax = Vector3.zero;
        //스케일을 1,1,1로
        result.localScale = Vector3.one;

        return result;
    }

    protected override IEnumerator Onconnected(GameManager newManager)
    {
        createdTransfrom = CreateFullScreen("CreatedUI");
        _movableScreen = CreateUI(UIType.Movable, "MovableScreen", MainCanvas?.transform);

        switcherTransform = CreateFullScreen("ScreenSwitcher");

        foreach(var currentPair in globalScreenArray)
        {
            UIBase created = CreateUI(currentPair.Key, currentPair.Value, switcherTransform);

            if (created is IOpenable asOpenable) asOpenable.Close();
        }

        changerTransform = CreateFullScreen("ScreenChanger");
        changerTransform.SetAsLastSibling();

        //screenChanger를 등록하는 과정
        for (ScreenChangeType currentChanger = (ScreenChangeType)1; currentChanger < ScreenChangeType._Length; currentChanger++) 
        {
            GameObject instance = ObjectManager.CreateObject(currentChanger.ToString(), changerTransform);
            if (instance?.TryGetComponent(out UI_ScreenChanger asChanger) ?? false)
            {
                screenChangerDictionary.Add(currentChanger, asChanger);
            }
            instance?.SetActive(false);
        }

        yield return null;
    }

    protected override void OnDisconnected()
    {
        UnsetAllUI();
    }

    protected void SetMainCanvas(Canvas newCanvas)
    {
        _mainCanvas = newCanvas;
        if (_mainCanvas)
        {
            _raycaster = _mainCanvas.GetComponent<GraphicRaycaster>();
            if(MainCanvas.transform is RectTransform mainRectTransform)
            {
                _uiScale = mainRectTransform.lossyScale.x;
                _uiBoundary = mainRectTransform.rect;  
                //_uiBoundary.size *= _uiScale;
                //_uiBoundary.position *= _uiScale / 1.0f;
            }
        }
        else
        {
            _raycaster = null;
        }
    }

    protected UIBase CreateUI(UIType wantType, string wantName, Transform parent)
    {
        GameObject instance = ObjectManager.CreateObject(wantName, parent);
        UIBase result = instance?.GetComponent<UIBase>();
        return SetUI(wantType, result);
    }
    protected UIBase CreateUI(UIType wantType, string wantName)
    {
        UIBase result = CreateUI(wantType, wantName, createdTransfrom ?? MainCanvas?.transform);
        if (result?.GetComponentInChildren<UI_DraggableWindow>())
        {
            _movableScreen?.SetChild(result.gameObject);
        }

        return result;
    }

    public static UIBase ClaimCreateUI(UIType wantType, string wantName) => GameManager.Instance?.UI?.CreateUI(wantType, wantName);

    protected UIBase SetUI(UIBase wantUI)
    {
        wantUI.Registration(this);
        return wantUI;
    }
    protected UIBase SetUI(UIType wantType, UIBase wantUI)
    {
        //들어온게 없다
        if (wantUI == null) return null;

        //이미 해당 타입은 등록되었으니, 원본을 주겠다
        if (uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin;

        //등록해준다
        uiDictionary.Add(wantType, wantUI);
        return SetUI(wantUI);
    }
    public static UIBase ClaimSetUI(UIBase wantUI)                  => GameManager.Instance?.UI?.SetUI(wantUI);
    public static UIBase ClaimSetUI(GameObject wantObject)          => ClaimSetUI(wantObject.GetComponent<UIBase>());
    public static UIBase ClaimSetUI(UIType wantType, UIBase wantUI) => GameManager.Instance?.UI?.SetUI(wantType, wantUI);

    protected void UnsetUI(UIType wantType)
    {
        if(uiDictionary.TryGetValue(wantType, out UIBase found))
        {
            UnsetUI(found);
            uiDictionary.Remove(wantType);
        }
    }
    protected void UnsetUI(UIBase wantUI)
    {
        if (!wantUI) return;

        wantUI.Unregistration(this);
    }
    public static void ClaimUnsetUI(UIBase wantUI)                  => GameManager.Instance?.UI?.UnsetUI(wantUI);
    public static void ClaimUnsetUI(GameObject wantObject)          => ClaimUnsetUI(wantObject?.GetComponent<UIBase>());
    protected void UnsetAllUI()
    {
        foreach(UIBase ui in uiDictionary.Values)
        {
            UnsetUI(ui); 
        }
        uiDictionary.Clear();
    }

    protected UIBase GetUI(UIType wantType)
    {
        if (uiDictionary.TryGetValue(wantType, out UIBase result)) return result;
        else return null;
    }
    public static UIBase ClaimGetUI(UIType wantType)                => GameManager.Instance?.UI?.GetUI(wantType);

    protected bool IsOpen(UIType wantType, out IOpenable resultOpenable)
    {
        resultOpenable = default;
        UIBase target = GetUI(wantType);

        if (!target) return false;
        resultOpenable = target as IOpenable;
        if (resultOpenable is not null) return resultOpenable.IsOpen;
        return target.gameObject.activeSelf;
    }
    protected bool CloseUI(params UIType[] wantTypes)
    {
        foreach (UIType wantType in wantTypes)
        {
            if (IsOpen(wantType, out IOpenable resultOpenable))
            {
                if (resultOpenable is null) continue;
                resultOpenable.Close();
                return true;
            }
        }
        return false;
    }
    public static bool ClaimCloseUI(params UIType[] wantTypes)      => GameManager.Instance?.UI?.CloseUI(wantTypes) ?? false;

    public static bool ClaimCheckOpen(UIType wantType, out IOpenable resultOpenable)
    {
        resultOpenable = default;
        return GameManager.Instance?.UI?.IsOpen(wantType, out resultOpenable) ?? false;
    }
    protected UIBase OpenUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        //result가 IOpenable을 상속받는 것을 어떻게 알 수 있을 것인가?
        //result는 IOpenable인 opener인가? 그렇다면 Open()을 실행하라
        if(result is IOpenable asOpenable) asOpenable.Open();

        if (result) EventSystem.current.SetSelectedGameObject(result.gameObject);

        //위랑 아래랑 같은 의미
        //IOpenable opener = result as IOpenable;
        //if (opener != null) opener.Open();
        return result;
    }
    public static UIBase ClaimOpenUI(UIType wantType)               => GameManager.Instance?.UI?.OpenUI(wantType);
    protected UIBase CloseUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Close();
        return result;
    }
    public static UIBase ClaimCloseUI(UIType wantType)              => GameManager.Instance?.UI?.CloseUI(wantType);
    protected UIBase ToggleUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Toggle();
        return result;
    }
    public static UIBase ClaimToggleUI(UIType wantType)             => GameManager.Instance?.UI?.ToggleUI(wantType);

    protected UIBase OpenScreen(UIType wantType)
    {
        CloseUI(CurrentScreen); //원래 있던거 닫기
        _currentScreenType = wantType; //갱신
        return OpenUI(wantType); //열기
    }
    public static UIBase ClaimOpenScreen(UIType wantType)           => GameManager.Instance?.UI?.OpenScreen(wantType);
    protected void OpenScreen(UIType wantScreen, ScreenChangeType changeType)
    {
        ClaimScreenChangeEffect(changeType, () => OpenScreen(wantScreen));
    }
    public static void ClaimOpenScreen(UIType wantScreen, ScreenChangeType changeType)
        => GameManager.Instance?.UI?.OpenScreen(wantScreen, changeType);

    protected void ScreenChangeEffectStart(ScreenChangeType wantType, Action endFunction = null)
    {
        //EventSystem.current.enabled = false;
        GameManager.Instance.Input.SetInputState(false);

        if (currentScreenChanger) return;

        if(screenChangerDictionary.TryGetValue(wantType, out UI_ScreenChanger result))
        {
            if(!result)
            {
                endFunction?.Invoke();
                return;
            }

            currentScreenChanger = result;
            result.gameObject.SetActive(true);
            result.ChangeStart(endFunction);
        }
        else
        {
            endFunction?.Invoke();
        }
    }
    public static void ClaimScreenChangeEffectStart(ScreenChangeType wantType, Action endFunction = null) 
        => GameManager.Instance?.UI?.ScreenChangeEffectStart(wantType, endFunction);
    public static void ClaimScreenChangeEffect(ScreenChangeType wantType, Action endFunction = null)
        => GameManager.Instance?.UI?.ScreenChangeEffectStart(wantType, endFunction + ClaimScreenChangeEffectEnd);
    protected void ScreenChangeEffectEnd()
    {
        if (!currentScreenChanger) return;
        GameObject targetObject = currentScreenChanger.gameObject;
        currentScreenChanger.ChangeEnd(() =>
        {
            targetObject.SetActive(false);
            GameManager.Instance.Input.SetInputState(true);
        });
        currentScreenChanger = null;

        //EventSystem.current.enabled = true;
    }
    public static void ClaimScreenChangeEffectEnd()                 => GameManager.Instance?.UI?.ScreenChangeEffectEnd();

    public static void ClaimPopUp(string title, string context, string confirm)
    {
        OnPopUp?.Invoke(title, context, confirm);
    }
    public static void ClaimErrorMessage(string context)
    {
        OnPopUp?.Invoke("Error", context, "Confirm");
    }
}
