using UnityEngine;

public class UI_Button_Save : MonoBehaviour
{
    public void OpenBattleScreen()
    {
        UIManager.ClaimOpenScreen(UIType.Battle);
    }
}
