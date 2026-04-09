using UnityEngine;

public class UI_Button_CloseWindow : MonoBehaviour
{
    [SerializeField] UIType wantType;

    public void Close()
    {
        UIManager.ClaimToggleUI(wantType);
    }
}
