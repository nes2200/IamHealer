using UnityEngine;

public class UI_Button_MenuWindowBackground : MonoBehaviour
{
    public void CloseMenuWindow()
    {
        UIManager.ClaimToggleUI(UIType.Menu);
        GameManager.UnPause();
    }
}
