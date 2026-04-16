using TMPro;
using UnityEngine;

public class CostStarStage : MonoBehaviour
{
    bool _isLimitOver = false;
    public bool IsLimitOver => _isLimitOver;

    [SerializeField] GameObject fullStar;
    [SerializeField] TextMeshProUGUI costLimitText;
    [SerializeField] Animator coststarAnimator;

    public void SetCostLimitText(int costLimit)
    {
        costLimitText.text = costLimit.ToString();
    }

    public void CostLimitOver()
    {
        coststarAnimator.SetTrigger("Over");
        _isLimitOver = true;
    }
    public void CostLimitRecover()
    {
        coststarAnimator.SetTrigger("Recover");
        _isLimitOver = false;
    }
}
