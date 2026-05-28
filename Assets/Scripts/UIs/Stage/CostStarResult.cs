using TMPro;
using UnityEngine;

public class CostStarResult : MonoBehaviour
{
    [SerializeField] GameObject fullStar;

    public void CostLimitOverCheck(bool isCostOver)
    {
        if(isCostOver) fullStar.SetActive(false);
        else fullStar.SetActive(true);
    }
}
