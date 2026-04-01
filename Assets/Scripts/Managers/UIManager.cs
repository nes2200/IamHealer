using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    None, Loading, Title, Movable,
    _Length
}

//팝업이 일어나는 이벤트가 발생할 것이다
//델리게이트 => 스킬을 무한히 배울 수 있음
//A스킬과 B스킬을 가르쳐 놨다 => 동시에 실행시키면 => 맨 마지막 결과만 알려준다
public delegate void PopUpEvent(string title, string context, string confirm);

public class UIManager : ManagerBase
{
    public static event PopUpEvent OnPopUp;

    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    Dictionary<UIType, UIBase> uiDictionary = new();

    public IEnumerator Initialize(GameManager newManager)
    {
        _mainCanvas = GetComponentInChildren<Canvas>();
        //GameObject.FindGameObjectsWithTag("MainCavas");
        SetUI(UIType.Loading, GetComponentInChildren<UI_LoadingScreen>());
        yield return null;
    }
    protected override IEnumerator Onconnected(GameManager newManager)
    {
        UIBase movableUI = CreateUI(UIType.Movable, "MovableScreen");
        yield return null;
        movableUI.SetChild(ObjectManager.CreateObject("PoPUp"));
        yield return null;
    }

    protected override void OnDisconnected()
    {
    }


    protected UIBase CreateUI(UIType wantType, string wantName)
    {
        GameObject instance = ObjectManager.CreateObject(wantName, _mainCanvas.transform);
        UIBase result = instance?.GetComponent<UIBase>();
        return SetUI(wantType, result);
    }
    protected UIBase SetUI(UIType wantType, UIBase wantUI)
    {
        //들어온게 없다
        if (wantUI == null) return null;

        //이미 해당 타입은 등록되었으니, 원본을 주겠다
        if (uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin;

        //등록해준다
        uiDictionary.Add(wantType, wantUI);
        return wantUI;
    }
    public static UIBase ClaimSetUI(UIType wantType, UIBase wantUI) => GameManager.Instance?.UI?.SetUI(wantType, wantUI);
    protected UIBase GetUI(UIType wantType)
    {
        if (uiDictionary.TryGetValue(wantType, out UIBase result)) return result;
        else return null;
    }
    public static UIBase ClaimGetUI(UIType wantType)     => GameManager.Instance?.UI?.GetUI(wantType);
    protected UIBase OpenUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        //result가 IOpenable을 상속받는 것을 어떻게 알 수 있을 것인가?
        //result는 IOpenable인 opener인가? 그렇다면 Open()을 실행하라
        if(result is IOpenable asOpenable) asOpenable.Open();

        //위랑 아래랑 같은 의미
        //IOpenable opener = result as IOpenable;
        //if (opener != null) opener.Open();


        return result;
    }
    public static UIBase ClaimOpenUI(UIType wantType)    => GameManager.Instance?.UI?.OpenUI(wantType);
    protected UIBase CloseUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Close();
        return result;
    }
    public static UIBase ClaimCloseUI(UIType wantType)   => GameManager.Instance?.UI?.CloseUI(wantType);
    protected UIBase ToggleUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Toggle();
        return result;
    }
    public static UIBase ClaimToggleUI(UIType wantType)  => GameManager.Instance?.UI?.ToggleUI(wantType);

    public static void ClaimPopUp(string title, string context, string confirm)
    {
        OnPopUp?.Invoke(title, context, confirm);
    }
    public static void ClaimErrorMessage(string context)
    {
        OnPopUp?.Invoke("Error", context, "Confirm");
    }


}
