using TMPro;
using UnityEngine;

public class UI_SaveLoadScreen : UI_ScreenBase
{
    public UI_SaveSlot saveSlot;

  
    public override void Open()
    {
        gameObject.SetActive(true);
        saveSlot.ChangeText();
    }
   
}
