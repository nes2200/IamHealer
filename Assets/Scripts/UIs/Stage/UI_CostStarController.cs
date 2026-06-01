using TMPro;
using UnityEngine;

public class UI_CostStarController : MonoBehaviour
{
    [Header("현재 코스트 텍스트")]
    [SerializeField] TextMeshProUGUI currentCostText;
    
    [Header("코스트 별들")]
    [SerializeField] UI_CostStarStage[] coststars;

    private void OnEnable()
    {
        CostTracker.OnCostChange -= CurrentCostTextChange;
        CostTracker.OnCostChange += CurrentCostTextChange;
    }
    private void OnDisable()
    {
        CostTracker.OnCostChange -= CurrentCostTextChange;
    }

    private void CurrentCostTextChange(int currentCost)
    {
        currentCostText.text = currentCost.ToString();
    }

    public void SetAllCostLimitText(int[] costLimits)
    {
        for (int i = 0; i < costLimits.Length; i++)
        {
            coststars[i].SetCostLimitText(costLimits[i]);
        }
    }

}
