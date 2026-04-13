using TMPro;
using UnityEngine;

public class UI_SaveLoadScreen : UIBase, IOpenable
{
    public UI_SaveSlot saveSlot;

    public bool IsOpen => gameObject.activeSelf;
    public void Open()
    {
        gameObject.SetActive(true);
        saveSlot.ChangeText();
    }
    public void Close() => gameObject.SetActive(false);
    public void Toggle() => gameObject.SetActive(!IsOpen);
}
