using TMPro;
using UnityEngine;

public class UI_SaveLoadScreen : UI_ScreenBase
{
    public UI_SaveSlot saveSlot;

    void BackToTitle(bool value) 
    {
        UIManager.ClaimOpenScreen(UIType.Title, ScreenChangeType.ScreenChanger);
    }

    public override void Open()
    {
        gameObject.SetActive(true);
        saveSlot.ChangeText();

        InputManager.OnCancel -= BackToTitle;
        InputManager.OnCancel += BackToTitle;
    }
    public override void Close()
    {
        InputManager.OnCancel -= BackToTitle;
        base.Close();
    }
}
