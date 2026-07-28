using Google.MiniJSON;
using TMPro;
using UnityEngine;

public class UI_Button_StageSelect : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageText;

    [Range(1, 5)]
    [SerializeField]int stage;
    [SerializeField] TextAsset stageDataJson;    

    public void SetStageText(int chapter)
    {
        stageText.text = $"{chapter}-{stage}";
    }

    public void ChangeSceneToStage()
    {
       GameManager.SceneLoad.LoadSceneAndSetup("StageScene", stageDataJson);
    }

}
