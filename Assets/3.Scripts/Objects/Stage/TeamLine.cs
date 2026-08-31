using UnityEngine;

public class TeamLine : MonoBehaviour
{
    private void OnEnable()
    {
        StageManager.OnBattleStart -= SetActiveFalse;
        StageManager.OnBattleStart += SetActiveFalse;
    }

    private void OnDisable()
    {
        StageManager.OnBattleStart -= SetActiveFalse;
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
