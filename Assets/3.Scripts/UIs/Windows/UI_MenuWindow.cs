using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_MenuWindow : OpenableUIBase
{
    public void RestartStage()
    {
        UIManager.ClaimCloseUI(UIType.Menu);
        UI_StageScreen.ClaimOnMenuClose();

        GameManager.ResetBattle();
        GameManager.UnPause();

        GameManager.SceneLoad.RestartCurrentStage();
    }


}
