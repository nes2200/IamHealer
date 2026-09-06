using TMPro;
using UnityEngine;

public class UI_CostStarStage : MonoBehaviour
{
    [SerializeField] GameObject fullStar;
    [SerializeField] TextMeshProUGUI costLimitText;
    [SerializeField] Animator starAnimator;

    int costLimit;
    bool isLimitOver = false;
    public bool IsLimitOver => isLimitOver;


    private void OnEnable()
    {
        CostTracker.OnCostChange -= CheckCostLimitOver;
        CostTracker.OnCostChange += CheckCostLimitOver;
    }
    private void OnDisable()
    {
        CostTracker.OnCostChange -= CheckCostLimitOver;
    }

    public void SetCostLimitText(int costLimit)
    {
        this.costLimit = costLimit;
        costLimitText.text = this.costLimit.ToString();
    }

    public void RefreshState(int currentCost)
    {
        isLimitOver = currentCost > costLimit;

        starAnimator.ResetTrigger("Over");
        starAnimator.ResetTrigger("Recover");
        starAnimator.Play(isLimitOver ? "Base Layer.CostOver" : "Base Layer.CostRecover", 0, 1f);
    }

    public void CheckCostLimitOver(int currentCost)
    {
        //현재 코스트가 코스트 리미트를 넘었는데, 아직 리미트가 안넘은 상태였다면?
        //그러니까 이번 변화로 인해 코스트 리미트를 넘었다면?
        if(currentCost > costLimit && !isLimitOver)
        {
            isLimitOver = true;
            starAnimator.SetTrigger("Over");
        }
        //이번 변화로 인해 코스트 리미트 안으로 들어왔다면?
        else if(currentCost <= costLimit && isLimitOver)
        {
            isLimitOver = false;
            starAnimator.SetTrigger("Recover");
        }
    }
}
