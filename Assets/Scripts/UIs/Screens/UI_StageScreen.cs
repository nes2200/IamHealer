using System;
using UnityEngine;

public class UI_StageScreen : UI_ScreenBase
{
    StageManager stageManager;

    [Header("별 그룹")]
    [SerializeField] UI_CostStarController starController;

    [Header("별 프리팹")]
    [SerializeField] GameObject starPrefab;

    [Header("유닛/무기 선택 영역")]
    [SerializeField] UI_UnitSelectArea unitSelectArea;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);

        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        SetCostLimitText(stageManager.GetCostLimits());

        InputManager.OnCancel -= ToggleMenu;
        InputManager.OnCancel += ToggleMenu;

        StageManager.OnStageStateChange -= OpenBattleResult;
        StageManager.OnStageStateChange += OpenBattleResult;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);

        InputManager.OnCancel -= ToggleMenu;
        StageManager.OnStageStateChange -= OpenBattleResult;
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

    private void SetCostLimitText(int[] costLimits)
    {
        starController.SetAllCostLimitText(costLimits);
    }

    private void OpenBattleResult(StageState oldState, StageState newState)
    {
        if (newState == StageState.Result)
        {
            UIBase instance = UIManager.ClaimOpenUI(UIType.BattleResult);
            UI_BattleResultWindow resultWindow = instance.GetComponent<UI_BattleResultWindow>();
            resultWindow.CostLimitOverCheck(starController.GetStageResult());
        }
    }
}
