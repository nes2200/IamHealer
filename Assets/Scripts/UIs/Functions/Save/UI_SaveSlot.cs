using TMPro;
using UnityEngine;

public class UI_SaveSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI saveText;
    [SerializeField] GameObject saveLayout;

    bool _isSave;
    public bool IsSave { get { return _isSave; } set { _isSave = value; } }

    public void ChangeText()
    {
        if( _isSave)
        {
            saveText.text = "저장하기";
        }
        else
        {
            saveText.text = "불러오기";
        }
        ChangeSaveText();
    }
    public void ChangeSaveText()
    {
        if (_isSave)
        {
            foreach(UI_Save save in saveLayout.GetComponentsInChildren<UI_Save>())
            {
                save.ChangeText("Save");
            }
        }
        else
        {
            foreach (UI_Save save in saveLayout.GetComponentsInChildren<UI_Save>())
            {
                save.ChangeText("Load");
            }
        }
    }
}
