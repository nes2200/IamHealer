using TMPro;
using UnityEngine;

public class UI_Save : MonoBehaviour
{
    public TextMeshProUGUI saveText;

    public void ChangeText(string wantText)
    {
        saveText.text = wantText;
    }
}
