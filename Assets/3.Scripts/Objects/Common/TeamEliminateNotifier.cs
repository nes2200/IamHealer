using UnityEngine;
using System.Collections.Generic;


public class TeamEliminateNotifier : MonoBehaviour
{
    [Header("Stage Manager")]
    [SerializeField] StageManager stageManager;
    [Header("Player Check")]
    [SerializeField] bool isPlayer;

    List<CharacterBase> teamCharacters;


    private void OnEnable()
    {
        StageManager.OnStageStateChange -= CacheChildrensCharacter;
        StageManager.OnStageStateChange += CacheChildrensCharacter;

        teamCharacters = new List<CharacterBase>();
    }
    private void OnDisable()
    {
        StageManager.OnStageStateChange -= CacheChildrensCharacter;
    }

    //자식들 캐싱하기
    public void CacheChildrensCharacter(StageState oldState, StageState newState)
    {
        if(newState == StageState.Battle)
        {
            teamCharacters.Clear();
            foreach (Transform child in transform)
            {
                CharacterBase character = child.GetComponent<CharacterBase>();
                if (character)
                {
                    teamCharacters.Add(character);
                }
            }
        }
    }

    //내 팀 유닛들이 전부 죽었는지 체크
    public void TeamEliminateCheck()
    {
        for(int i = 0; i < teamCharacters.Count - 1; i++)
        {
            if (teamCharacters[i].IsAlive) return;
        }

        TeamEliminated();
    }

    private void TeamEliminated()
    {
        stageManager.EndBattle(isPlayer);
    }
}
