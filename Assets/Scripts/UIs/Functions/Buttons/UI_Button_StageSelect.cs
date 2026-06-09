using TMPro;
using UnityEngine;

public class UI_Button_StageSelect : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageText;

    int chapter;

    public void SetStageText(int chapter, int stage)
    {
        stageText.text = $"{chapter}-{stage}";
    }

    public void OpenStage()
    {
        UIManager.ClaimOpenScreen(UIType.Stage, ScreenChangeType.SlideChanger);
    }
}
