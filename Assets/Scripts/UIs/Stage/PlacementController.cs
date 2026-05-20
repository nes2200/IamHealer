using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementController : MonoBehaviour
{
    [Header("유닛 생성 세팅")]
    [SerializeField] GameObject unitPrefab;
    [SerializeField] LayerMask floorLayer;

    Transform teamA_Parent;
    Transform teamB_Parent;

    private void OnEnable()
    {
        InputManager.OnMouseLeftButton -= TryUnitSpawn;
        InputManager.OnMouseLeftButton += TryUnitSpawn;

        SetUnitsParent(ref teamA_Parent, "TeamA");
        SetUnitsParent(ref teamB_Parent, "TeamB");
    }
    private void OnDisable()
    {
        InputManager.OnMouseLeftButton -= TryUnitSpawn;
    }

    private void SetUnitsParent(ref Transform parent, string name)
    {
        if (!parent)
        {
            GameObject findObj = GameObject.Find(name);
            if (findObj)
            {
                parent = findObj.transform;
            }
            else
            {
                Debug.Log($"{name}을/를 찾을 수 없음");
            }
        }
    }

    private void TryUnitSpawn(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        //마우스 뗄떼는 유닛 생성 안함
        if (!value) return;

        //마우스가 지금 가리키는 오브젝트 가져오기
        GameObject mouseOverObj = GameManager.Instance.Input.CursorHoverObject;

        //없으면 유닛 생성 안함
        if (!mouseOverObj) return;

        //바닥이 아니면 생성 안함
        if(mouseOverObj.layer != LayerMask.NameToLayer("Floor")) return;

        //바닥에 맞았으면 유닛 생성
        GameObject newUnit = ObjectManager.CreateObject("Unit", worldPosition, teamA_Parent);

        //생성됬으면 등록하기
        if (newUnit)
        {
            //생성한놈 등록하기
            ObjectManager.RegistrationObject(newUnit);
            //추적할 적 유닛 등록하기
            TargetingModule targetModule = newUnit.GetComponent<TargetingModule>();
            if (targetModule)
            {
                targetModule.SetHostileGroupPanrets(teamB_Parent);
            }
        }

        ////UI 위에서는 유닛 생성 안함
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}

        ////빔 쏘기
        //Ray ray = GameManager.Instance.Camera.MainCamera.ScreenPointToRay(screenPosition);
        //RaycastHit hit;

        ////빔 쏜게 바닥에 맞았는지 체크
        //if(Physics.Raycast(ray, out hit, float.MaxValue, floorLayer))
        //{

        //}
    }
}