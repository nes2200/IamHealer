using TMPro;
using UnityEngine;

//버튼들은 자신들이 어느 챕터인지 알아야 함
//선택되면, 자신의 챕터 스테이지로 가야함

public class UI_Button_ChapterSelect : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chapterNumberText;

    [Range(1,5)]
    [SerializeField] int chapter = 1;

    private void OnEnable()
    {
        chapterNumberText.text = chapter.ToString();
    }

    public void ChapterSelected()
    {
        UIManager.ClaimOpenScreen(UIType.StageSelect, ScreenChangeType.ScreenChanger);
        UI_StageSelectScreen stageSelectScreen = UIManager.ClaimGetUI(UIType.StageSelect) as UI_StageSelectScreen;
        stageSelectScreen.SetChapter(chapter);
    }
}
