using UnityEngine;

public class UI_OptionScreen : UI_ScreenBase
{
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
    }
    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
    }

    public override void Open()
    {
        base.Open();
        InputManager.OnCancel -= BackToTitle;
        InputManager.OnCancel += BackToTitle;
    }
    public override void Close()
    {
        InputManager.OnCancel -= BackToTitle;
        base.Close();
    }

    void BackToTitle(bool value) => UIManager.ClaimOpenScreen(UIType.Title, ScreenChangeType.ScreenChanger);
}
