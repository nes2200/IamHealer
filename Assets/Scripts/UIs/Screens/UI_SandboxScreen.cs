using UnityEngine;

public class UI_SandboxScreen : UI_ScreenBase
{
    private void OnEnable()
    {
        InputManager.OnCancel -= BackToTitle;
        InputManager.OnCancel += BackToTitle;

    }
    private void OnDisable()
    {
        InputManager.OnCancel -= BackToTitle;
    }

    void BackToTitle(bool value) => UIManager.ClaimOpenScreen(UIType.Title, ScreenChangeType.ScreenChanger);
}
