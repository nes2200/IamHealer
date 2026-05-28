using UnityEngine;

public class UI_Button_BattleStart : MonoBehaviour
{
    StageController stage;

    private void OnEnable()
    {
        stage = GameObject.Find("BattleManager").GetComponent<StageController>();
    }

    public void BattleStart()
    {
        GameManager.StartBattle();
        
        if(stage != null)
        {
            stage.StartBattle();
        }
    }
}
