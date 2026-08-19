using UnityEngine;

public class UI_StageScreen : UI_ScreenBase
{
    public static event System.Action OnMenuOpen;
    public static event System.Action OnMenuClose;


    StageManager stageManager;

    [Header("별 그룹")]
    [SerializeField] UI_CostStarController starController;

    [Header("별 프리팹")]
    [SerializeField] GameObject starPrefab;

    [Header("HPBar Group")]
    [SerializeField] UI_HPBarGroup hpBarGroup;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        GameManager.StageLoad.OnStageLoaded -= ConnectStage;
        GameManager.StageLoad.OnStageLoaded += ConnectStage;
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        GameManager.StageLoad.OnStageLoaded -= ConnectStage;

        hpBarGroup.Disconnect();
        hpBarGroup.ClearAllHPBars();
    }

    public override void Open()
    {
        base.Open();

        GameManager.Camera.AddCameraController();
        GameManager.ResetBattle();

        InputManager.OnCancel -= ToggleMenu;
        InputManager.OnCancel += ToggleMenu;

        StageManager.OnBattleEnd -= OpenBattleResult;
        StageManager.OnBattleEnd += OpenBattleResult;
    }
    public override void Close()
    {
        base.Close();
        GameManager.Camera.RemoveCameraController();
        GameManager.ResetBattle();

        InputManager.OnCancel -= ToggleMenu;
        StageManager.OnBattleEnd -= OpenBattleResult;

        hpBarGroup.ClearAllHPBars();
    }

    public void ToggleMenu(bool value)
    {
        UIManager.ClaimToggleUI(UIType.Menu);

        bool isMenuOpen = UIManager.ClaimCheckOpen(UIType.Menu, out _);
        if (isMenuOpen)
        {
            GameManager.Pause();
            OnMenuOpen?.Invoke();
        }
        else
        {
            GameManager.UnPause();
            OnMenuClose?.Invoke();
        }
    }

    public static void ClaimOnMenuClose()
    {
        OnMenuClose?.Invoke();
    }

    private void SetCostLimitText(int[] costLimits)
    {
        starController.SetAllCostLimitText(costLimits);
    }

    private void OpenBattleResult(bool isPlayerLoose)
    {
        UIBase instance = UIManager.ClaimOpenUI(UIType.BattleResult);
        UI_BattleResultWindow resultWindow = instance as UI_BattleResultWindow;

        resultWindow.SetResult(isPlayerLoose, starController.GetStageResult());
    }

    private void ConnectStage(StageManager newStageManager)
    {
        stageManager = newStageManager;
        hpBarGroup.Connect(stageManager.CharacterRegistry);
        SetCostLimitText(stageManager.GetCostLimits());
    }
}
