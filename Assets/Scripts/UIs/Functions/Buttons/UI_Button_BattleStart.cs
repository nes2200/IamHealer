using UnityEngine;

public class UI_Button_BattleStart : MonoBehaviour
{
    [SerializeField] CostChecker costChecker;

    public void BattleResultWindowOpen()
    {
        UIBase instance = UIManager.ClaimOpenUI(UIType.BattleResult);
        UI_BattleResultWindow resultWindow = instance.GetComponent<UI_BattleResultWindow>();
        bool[] costOverResult = costChecker.CostLimitOverResult();
        resultWindow.CostLimitOverCheck(costOverResult);
    }
}
