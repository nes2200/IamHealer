using System;
using UnityEngine;

[Serializable]
public struct UIClaim
{
    public string prefabName;
    public UIType uiType;
    public bool   isOpen;

    public UIBase Execute()
    {
        //찾아보자
        UIBase result = UIManager.ClaimGetUI(uiType);
        //찾아보니 없다                    만들자
        if (!result) result = UIManager.ClaimCreateUI(uiType, prefabName);
        //만든게 없다         없네
        if (!result) return result;

        //대상이 오픈 가능하다면
        if(result is IOpenable openTarget)
        {
            if(isOpen) openTarget.Open();
            else openTarget.Close();
        }
        
        return result;
    }
}

public class UI_ScreenBase : OpenableUIBase
{
    [SerializeField] UIClaim[] requiredUI;
    [SerializeField] protected UIType[] closeWithScreen; 
 

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        if (requiredUI is null) return;

        foreach(UIClaim currentClaim in requiredUI)
        {
            currentClaim.Execute();
        }
    }

    public override void Close()
    {
        base.Close();

        if (closeWithScreen != null) 
        {
            foreach(UIType currentUI in closeWithScreen) UIManager.ClaimCloseUI(currentUI);
        }
    }

    public virtual bool CloseInnerUI() => UIManager.ClaimCloseUI(closeWithScreen);
}
