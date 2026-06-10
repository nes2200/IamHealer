using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [Header("스테이지 매니저")]
    [SerializeField] StageManager stageManager;

    [Header("유닛 생성 세팅")]
    [SerializeField] LayerMask floorLayer;

    GameObject unitPrefab;
    int selectedUnitCost;

    [Header("각 팀 부모")]
    [SerializeField] Transform teamA_Parent;
    [SerializeField] Transform teamB_Parent;

    private void OnEnable()
    {
        InputManager.OnMouseLeftButton -= TryUnitSpawn;
        InputManager.OnMouseLeftButton += TryUnitSpawn;

        InputManager.OnMouseRightButton -= TryUnitDespawn;
        InputManager.OnMouseRightButton += TryUnitDespawn;

        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;
        InputManager.OnUnitSelect += ChangeCurrentSelectedUnit;
    }
    private void OnDisable()
    {
        InputManager.OnMouseLeftButton -= TryUnitSpawn;
        InputManager.OnMouseRightButton -= TryUnitDespawn;
        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;

    }

    private void TryUnitSpawn(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        //마우스 뗄떼는 유닛 생성 안함
        if (!value) return;

        //준비 상태 아니라면 소환 안하기
        if (stageManager.CurrentState != StageState.Ready) return;

        //프리팹에 유닛이 저장되있지 않으면 생성 안함
        if(!unitPrefab) return;

        //생성할 유닛 비용이 추가될 시 리미트 최고점을 넘으면 생성 안함
        if (!IsCostEnoughToSpawn(selectedUnitCost)) return;

        //마우스가 지금 가리키는 오브젝트 가져오기
        GameObject mouseOverObj = InputManager.CursorHoverObject;

        //없으면 유닛 생성 안함
        if (!mouseOverObj) return;

        //바닥이 아니면 생성 안함
        if(mouseOverObj.layer != LayerMask.NameToLayer("Floor")) return;

        //위치가 생성 불가한 위치인지 체크하고 불가하면 안함
        if (worldPosition.x > 0) return;

        //바닥에 맞았으면 유닛 생성
        GameObject newUnit = ObjectManager.CreateObject(unitPrefab.name, worldPosition);

        //생성됬으면 등록하기
        if (newUnit)
        {
            Transform unitParent = teamA_Parent;
            //유닛의 부모 설정으로 팀 배정
            newUnit.transform.SetParent(unitParent);

            //생성한놈 등록하기
            ObjectManager.RegistrationObject(newUnit);
            //추적할 적 유닛 등록하기
            TargetingModule targetModule = newUnit.GetComponent<TargetingModule>();
            if (targetModule)
            {
                //부모가 A면 적은 B, 부모가 B면 적은 A
                targetModule.SetHostileGroupParents((unitParent == teamA_Parent) ? teamB_Parent : teamA_Parent);
            }

            CharacterBase targetCharacter = newUnit.GetComponent<CharacterBase>();
            if (targetCharacter)
            {
                int unitCost = targetCharacter.Status.cost;
                stageManager.CostIncreasByUnitSpawn(unitCost);
            }
        }
    }

    private void TryUnitDespawn(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;

        if (worldPosition.x > 0) return;

        GameObject mouseOverObj = InputManager.CursorHoverObject;

        if(!mouseOverObj) return;

        if (mouseOverObj.layer != LayerMask.NameToLayer("Character")) return;

        CharacterBase targetCharacter = mouseOverObj.GetComponent<CharacterBase>();
        if (targetCharacter)
        {
            int unitCost = targetCharacter.Status.cost;
            stageManager.CostDecreaseByUnitDespawn(unitCost);
            ObjectManager.DestroyObject(mouseOverObj);
        }
    }

    //private Transform SetTeamBySpawnPosition(Vector3 worldPosition)
    //{
    //    if(worldPosition.x < 0)
    //    {
    //        return teamA_Parent;
    //    }
    //    else
    //    {
    //        return teamB_Parent;
    //    }
    //}

    //유닛 선택 버튼 클릭시 해당 유닛 정보를 받아오는 기능
    public void ChangeCurrentSelectedUnit(GameObject newUnit, int unitCost)
    {
        unitPrefab = newUnit;
        selectedUnitCost = unitCost;
    }

    public bool IsCostEnoughToSpawn(int unitCost)
    {
        return stageManager.IsCostEnoughToSpawn(unitCost);
    }
}