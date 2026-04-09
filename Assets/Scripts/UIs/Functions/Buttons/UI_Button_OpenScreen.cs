using UnityEngine;

public class UI_Button_OpenScreen : MonoBehaviour
{
    [SerializeField] UIType wantType;
    public void OpenScreen()
    {
        UIManager.ClaimOpenScreen(wantType);
    }
    public void OpenScreenCloseMenu()
    {
        OpenScreen();
        UIManager.ClaimCloseUI(UIType.Menu);
    }
    public void OpenSaveScreen()
    {
    }
}
