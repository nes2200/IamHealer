using UnityEngine;

public class UI_BattleResultWindow : OpenableUIBase
{
    [SerializeField] Animator anim;

    [SerializeField] UI_CostStarResult[] stars;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
    }

    private void OnEnable()
    {
        anim.SetTrigger("Show");
    }


    public void CostLimitOverCheck(bool[] costLimitOverResult)
    {
        for(int i = 0; i < stars.Length; i++)
        {
            stars[i].CostLimitOverCheck(costLimitOverResult[i]);
        }
    }
}
