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
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
    }

    public override void Open()
    {
        base.Open();
        GameManager.Instance.Camera.AddCameraController();
        GameManager.ResetBattle();

        InputManager.OnCancel -= ToggleMenu;
        InputManager.OnCancel += ToggleMenu;

        StageManager.OnStageStateChange -= OpenBattleResult;
        StageManager.OnStageStateChange += OpenBattleResult;
    }
    public override void Close()
    {
        base.Close();
        GameManager.Instance.Camera.RemoveCameraController();
        GameManager.ResetBattle();

        InputManager.OnCancel -= ToggleMenu;
        StageManager.OnStageStateChange -= OpenBattleResult;
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
        if (newState != StageState.Result) return;

        UIBase instance = UIManager.ClaimOpenUI(UIType.BattleResult);
        UI_BattleResultWindow resultWindow = instance as UI_BattleResultWindow;
        resultWindow.CostLimitOverCheck(starController.GetStageResult());
        InputManager.OnCancel -= ToggleMenu;
    }
}
