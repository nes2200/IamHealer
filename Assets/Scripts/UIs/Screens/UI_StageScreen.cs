using TMPro;
using UnityEngine;

public class UI_StageScreen : UI_ScreenBase
{
    private void OnEnable()
    {
        InputManager.OnCancel -= ToggleMenu;
        InputManager.OnCancel += ToggleMenu;
    }
    private void OnDisable()
    {
        InputManager.OnCancel -= ToggleMenu;
    }
    void ToggleMenu(bool value) => UIManager.ClaimToggleUI(UIType.Menu);
}
