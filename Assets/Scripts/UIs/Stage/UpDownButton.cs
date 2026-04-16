using TMPro;
using UnityEngine;

public class UpDownButton : MonoBehaviour
{
    [SerializeField] CostChecker costChecker;

    public void UpButton()
    {
        costChecker.CurrentCostUp();
    }
    public void DownButton()
    {
        costChecker.CurrentCostDown();
    }
}
