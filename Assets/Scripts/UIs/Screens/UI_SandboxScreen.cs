using UnityEngine;

public class UI_SandboxScreen : UI_ScreenBase
{
    public override void Open()
    {
        base.Open();
        InputManager.OnCancel -= BackToTitle;
        InputManager.OnCancel += BackToTitle;
    }
    public override void Close()
    {
        base.Close();
        InputManager.OnCancel -= BackToTitle;
    }

    void BackToTitle(bool value) => UIManager.ClaimOpenScreen(UIType.Title, ScreenChangeType.ScreenChanger);
}
