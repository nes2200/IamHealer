using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;

public class CostChecker : MonoBehaviour
{
    int currentCost = 0;
    [SerializeField] int costLimitFirst = 50;
    [SerializeField] int costLimitSecond = 60;
    [SerializeField] int costLimitThird = 70;

    [SerializeField] TextMeshProUGUI currentCostText;

    [SerializeField] CostStarStage coststarFirst; 
    [SerializeField] CostStarStage coststarSecond; 
    [SerializeField] CostStarStage coststarThird; 

    private void Start()
    {
        currentCostText.text = $"{currentCost}";
        
        coststarFirst.SetCostLimitText(costLimitFirst);
        coststarSecond.SetCostLimitText (costLimitSecond);
        coststarThird.SetCostLimitText (costLimitThird);
    }

    public void CurrentCostUp()
    {
        if (currentCost == costLimitThird) return;

        currentCost += 5;
        currentCostText.text  = $"{currentCost}";
        CostLimitOverCheck();
    }
    public void CurrentCostDown()
    {
        if (currentCost == 0) return;
        
        currentCost -= 5;
        currentCostText.text = $"{currentCost}";
        CostLimitRecoverCheck();
    }

    protected void CostLimitOverCheck()
    {
        if(currentCost > costLimitFirst)
        {
            if (!coststarFirst.IsLimitOver) coststarFirst.CostLimitOver();
        }
        if (currentCost > costLimitSecond)
        {
            if (!coststarSecond.IsLimitOver) coststarSecond.CostLimitOver();
        }
        if (currentCost > costLimitThird)
        {
            if (!coststarThird.IsLimitOver) coststarThird.CostLimitOver();
        }
    }
    protected void CostLimitRecoverCheck()
    {
        if (currentCost <= costLimitFirst)
        {
            if (coststarFirst.IsLimitOver) coststarFirst.CostLimitRecover();
        }
        if (currentCost <= costLimitSecond)
        {
            if (coststarSecond.IsLimitOver) coststarSecond.CostLimitRecover();
        }
        if (currentCost <= costLimitThird)
        {
            if (coststarThird.IsLimitOver) coststarThird.CostLimitRecover();
        }
    }

    public bool[] CostLimitOverResult()
    {
        bool[] result = new bool[3];

        result[0] = coststarFirst.IsLimitOver;
        result[1] = coststarSecond.IsLimitOver;
        result[2] = coststarThird.IsLimitOver;

        return result;
    }
}
