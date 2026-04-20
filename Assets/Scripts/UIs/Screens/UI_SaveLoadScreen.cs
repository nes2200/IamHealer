using TMPro;
using UnityEngine;

public class UI_SaveLoadScreen : UI_ScreenBase
{
    public UI_SaveSlot saveSlot;

    private void OnEnable()
    {
        InputManager.OnCancel -= BackToTitle;
        InputManager.OnCancel += BackToTitle;

    }
    private void OnDisable()
    {
        InputManager.OnCancel -= BackToTitle;
    }

    void BackToTitle(bool value) 
    {
        UIManager.ClaimOpenScreen(UIType.Title, ScreenChangeType.ScreenChanger);
    }

    public override void Open()
    {
        gameObject.SetActive(true);
        saveSlot.ChangeText();
    }
   


}
