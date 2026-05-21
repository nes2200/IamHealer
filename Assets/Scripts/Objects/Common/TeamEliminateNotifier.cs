using UnityEngine;
using System.Collections.Generic;


public class TeamEliminateNotifier : MonoBehaviour
{
    [SerializeField] StageController stageController;

    List<CharacterBase> teamCharacters;

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    //자식들 캐싱하기
    public void CacheChildrensCharacter()
    {
        teamCharacters.Clear();
        foreach(Transform child in transform)
        {
            CharacterBase character = child.GetComponent<CharacterBase>();
            if (character)
            {
                teamCharacters.Add(character);
            }
        }
    }

    //내 팀 유닛들이 전부 죽었는지 체크
    public void TeamEliminateCheck()
    {
        for(int i = 0; i < teamCharacters.Count; i++)
        {
            if (teamCharacters[i].IsAlive) return;
        }


    }

    private void TeamEliminated()
    {

    }
}
