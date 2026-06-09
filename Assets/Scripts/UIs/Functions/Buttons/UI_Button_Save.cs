using UnityEngine;

public class UI_Button_Save : MonoBehaviour
{
    [SerializeField] UIType wantType;

    public void OpenScreen()
    {
        UIManager.ClaimOpenScreen(wantType, ScreenChangeType.ScreenChanger);
    }
}
