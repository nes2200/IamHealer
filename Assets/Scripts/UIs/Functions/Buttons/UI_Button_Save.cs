using UnityEngine;

public class UI_Button_Save : MonoBehaviour
{
    public void OpenStageScreen()
    {
        UIManager.ClaimOpenScreen(UIType.Stage, ScreenChangeType.ScreenChanger);
    }
}
