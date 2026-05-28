using System;
using UnityEngine;

public class UI_StageScreen : UI_ScreenBase
{
    [Header("UI 구성 요소")]
    [SerializeField] CostChecker costChecker;

    private void OnEnable()
    {
        InputManager.OnCancel -= ToggleMenu;
        InputManager.OnCancel += ToggleMenu;

        StageController.OnStageStateChange -= OpenBattleResult;
        StageController.OnStageStateChange += OpenBattleResult;
    }

    private void OnDisable()
    {
        InputManager.OnCancel -= ToggleMenu;
        StageController.OnStageStateChange -= OpenBattleResult;
    }

    public override void Open()
    {
        base.Open();
        GameManager.Instance.Camera.AddCameraController();
    }
    public override void Close()
    {
        base.Close();
        GameManager.Instance.Camera.RemoveCameraController();
    }

    public void ToggleMenu(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Menu);

        bool isMenuOpen = UIManager.ClaimCheckOpen(UIType.Menu, out _);
        if (isMenuOpen)
        {
            GameManager.Pause();
        }
        else
        {
            GameManager.UnPause();
        }
    }

    private void OpenBattleResult(StageState oldState, StageState newState)
    {
        if(newState == StageState.Result)
        {
            UIBase instance = UIManager.ClaimOpenUI(UIType.BattleResult);
            UI_BattleResultWindow resultWindow = instance.GetComponent<UI_BattleResultWindow>();
            bool[] costOverResult = costChecker.CostLimitOverResult();
            resultWindow.CostLimitOverCheck(costOverResult);
        }
    }
}
