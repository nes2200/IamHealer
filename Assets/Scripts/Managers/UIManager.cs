using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    None, Loading, Title,
    _Length
}

public class UIManager : ManagerBase
{
    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    Dictionary<UIType, UIBase> uiDictionary = new();

    protected override IEnumerator Onconnected(GameManager newManager)
    {
        _mainCanvas = GetComponentInChildren<Canvas>();
        //GameObject.FindGameObjectsWithTag("MainCavas");
        yield return null;
    }

    protected override void OnDisconnected()
    {
    }

    public UIBase SetUI(UIType wantType, UIBase wantUI)
    {
        //들어온게 없다
        if (wantUI == null) return null; 

        //이미 해당 타입은 등록되었으니, 원본을 주겠다
        if (uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin; 

        //등록해준다
        uiDictionary.Add(wantType, wantUI);
        return wantUI;
    }
    public UIBase GetUI(UIType wantType)
    {
        if (uiDictionary.TryGetValue(wantType, out UIBase result)) return result;
        else return null;
    }

    public UIBase OpenUI(UIType wantType)
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
    public UIBase CloseUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Close();
        return result;
    }
    public UIBase ToggleUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Toggle();
        return result;
    }

}
