using System;
using UnityEngine;

public class UI_MovableScreen : UIBase
{
    Vector3 popupPosition = Vector3.zero;
    Vector3 popupShift = new(15f,-15f);

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        UIManager.OnPopUp -= PopUp;
        UIManager.OnPopUp += PopUp;
    }
    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        UIManager.OnPopUp -= PopUp; 
    }

    protected override GameObject OnSetChild(GameObject newChild)
    {
        //새로운 자식에게 UIManager가서 등록받아오라 시킨다
        if (newChild)
        {
            UIManager.ClaimSetUI(newChild);
        }
        return base.OnSetChild(newChild);
    }
    protected override void OnUnsetChild(GameObject oldChild)
    {
        UIManager.ClaimUnsetUI(oldChild);
        base.OnUnsetChild(oldChild);
    }

    private void PopUp(string title, string context, string confirm)
    {
        GameObject newChild = SetChild(ObjectManager.CreateObject("PopUp"));
        if (newChild)
        {
            if(newChild.TryGetComponent(out ISystemMessagePossible target))
            {
                target.SetSystemMessage(title, context, confirm);
            }
            if(newChild.TryGetComponent(out IConfirmable confirmTarget))
            {
                confirmTarget.SetConfirmAction(() => // 팝업창을 누르면
                {
                    UnsetChild(newChild); //자식에서 제외하고
                    ObjectManager.DestroyObject(newChild); //파괴함
                });
            }

            newChild.transform.localPosition = popupPosition;
            popupPosition += popupShift;
        }
    }
}
