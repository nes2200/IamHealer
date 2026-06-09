using TMPro;
using UnityEngine;

public class UI_StageSelectScreen : UI_ScreenBase
{
    [SerializeField] TextMeshProUGUI chapterText;
    [SerializeField] UI_Button_StageSelect[] stageButtons;
    int chapter;

    public override void Open()
    {
        base.Open();
        InputManager.OnCancel -= BackToChapter;
        InputManager.OnCancel += BackToChapter;

    }
    public override void Close()
    {
        InputManager.OnCancel -= BackToChapter;
        base.Close();
    }

    void BackToChapter(bool value) => UIManager.ClaimOpenScreen(UIType.ChapterSelect, ScreenChangeType.ScreenChanger);

    public void SetChapter(int chapter)
    {
        this.chapter = chapter;
        chapterText.text = $"{chapter}ц╘ем";

        for(int i = 0; i < stageButtons.Length; i++)
        {
            stageButtons[i].SetStageText(chapter, i + 1);
        }
    }

   
}
