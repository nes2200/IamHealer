using UnityEngine;

public class UI_TitleScreen : UI_ScreenBase
{
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnCancel -= ToggleCloseConfirm;
        InputManager.OnCancel += ToggleCloseConfirm;
    }
    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        InputManager.OnCancel -= ToggleCloseConfirm;
    }

    public override void Open()
    {
        base.Open();
        InputManager.OnCancel -= ToggleCloseConfirm;
        InputManager.OnCancel += ToggleCloseConfirm;
    }
    public override void Close()
    {
        base.Close();
        InputManager.OnCancel -= ToggleCloseConfirm;
    }

    void ToggleCloseConfirm(bool value) => UIManager.ClaimToggleUI(UIType.GameQuit);
}
