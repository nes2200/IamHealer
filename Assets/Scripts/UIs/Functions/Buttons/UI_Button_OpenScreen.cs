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
        UI_SaveLoadScreen saveScreen = UIManager.ClaimGetUI(UIType.SaveSlot) as UI_SaveLoadScreen;
        saveScreen.saveSlot.IsSave = true;
        OpenScreen();
    }
    public void OpenLoadScreen()
    {
        UI_SaveLoadScreen saveScreen = UIManager.ClaimGetUI(UIType.SaveSlot) as UI_SaveLoadScreen;
        saveScreen.saveSlot.IsSave = false;
        OpenScreen();
    }
}
