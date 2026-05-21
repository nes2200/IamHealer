using UnityEngine;

public class UI_Button_BattleStart : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] CostChecker costChecker;

    StageController stage;

    private void OnEnable()
    {
        stage = GameObject.Find("BattleManager").GetComponent<StageController>();
    }

    public void BattleStart()
    {
        GameManager.StartBattle();
        
        if(stage != null)
        {
            stage.StartBattle();
        }
    }

    public void BattleResultWindowOpen()
    {
        UIBase instance = UIManager.ClaimOpenUI(UIType.BattleResult);
        UI_BattleResultWindow resultWindow = instance.GetComponent<UI_BattleResultWindow>();
        bool[] costOverResult = costChecker.CostLimitOverResult();
        resultWindow.CostLimitOverCheck(costOverResult);
    }
}
