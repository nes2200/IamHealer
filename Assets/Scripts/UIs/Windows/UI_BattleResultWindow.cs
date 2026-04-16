using UnityEngine;

public class UI_BattleResultWindow : OpenableUIBase
{
    [SerializeField] Animator anim;
    [SerializeField] CostStarResult coststarFirst;
    [SerializeField] CostStarResult coststarSecond;
    [SerializeField] CostStarResult coststarThird;

    private void OnEnable()
    {

        anim.SetTrigger("Show");
    }

    public void CostLimitOverCheck(bool[] costLimitOverResult)
    {
        coststarFirst.CostLimitOverCheck(costLimitOverResult[0]);
        coststarSecond.CostLimitOverCheck(costLimitOverResult[1]);
        coststarThird.CostLimitOverCheck(costLimitOverResult[2]);
    }
}
