using UnityEngine;

public class UI_TitleScreen : UI_ScreenBase
{
    private void OnEnable()
    {
        InputManager.OnCancel -= ToggleCloseConfirm;
        InputManager.OnCancel += ToggleCloseConfirm;
    }
    private void OnDisable()
    {
        InputManager.OnCancel -= ToggleCloseConfirm;

    }
    void ToggleCloseConfirm(bool value) => UIManager.ClaimToggleUI(UIType.GameQuit);
}
