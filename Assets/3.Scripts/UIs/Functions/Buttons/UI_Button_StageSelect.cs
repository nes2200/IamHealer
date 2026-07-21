using TMPro;
using UnityEngine;

public class UI_Button_StageSelect : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageText;

    [Range(1, 5)]
    [SerializeField]int stage;

    public void SetStageText(int chapter)
    {
        stageText.text = $"{chapter}-{stage}";
    }

    public void ChangeSceneToStage()
    {
       GameManager.Instance.LoadSceneAndSetup("StageScene");
    }

}
