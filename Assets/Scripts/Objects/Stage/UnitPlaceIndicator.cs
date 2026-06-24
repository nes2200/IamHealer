using UnityEngine;
using UnityEngine.AI;

public class UnitPlaceIndicator : MonoBehaviour
{
    [Header("Offsets")]
    [SerializeField] float floorOffset = 0.025f;
    [SerializeField] float navMeshCheckRadius = 0.1f;

    [Header("Indicator")]
    [SerializeField] GameObject indicator;
    [SerializeField] Material indicatorMat;

    float size;
    bool selected = false;
    bool canSpawn;

    private void OnEnable()
    {
        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;
        InputManager.OnUnitSelect += ChangeCurrentSelectedUnit;

        InputManager.OnMouseMove -= MoveToMouse;
        InputManager.OnMouseMove += MoveToMouse;

        StageManager.OnBattleStart -= DisablePreview;
        StageManager.OnBattleStart += DisablePreview;

        //처음에는 선택된 유닛이 없으니까
        SetIndicatorVisual(false);

    }
    private void OnDisable()
    {
        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;
        InputManager.OnMouseMove -= MoveToMouse;
        StageManager.OnBattleStart -= DisablePreview;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!selected) return;

        //비정상값 -> 제외
        if (worldPosition.y < -9000f)
        {
            SetIndicatorVisual(false);
            canSpawn = false;
            return;
        }

        CheckSpawnable(worldPosition);

        SetIndicatorVisual(true);
        transform.position = worldPosition + new Vector3 (0f, floorOffset, 0f);
    }

    void CheckSpawnable(Vector3 worldPosition)
    {
        bool SpawnableCheck = NavMesh.SamplePosition(worldPosition, out NavMeshHit hit, navMeshCheckRadius, NavMesh.AllAreas);

        if(canSpawn != SpawnableCheck)
        {
            if (SpawnableCheck)
            {
                Debug.Log("스폰 가능");
                indicatorMat.SetColor("_TintColor", new Color(0f, 1f, 0f, 0.6f));
            }
            else
            {
                Debug.Log("불가능");
                indicatorMat.SetColor("_TintColor", new Color(1f, 0f, 0f, 0.6f));
            }
            canSpawn = SpawnableCheck;
        }
    }

    void SetIndicatorVisual(bool visible)
    {
        if (indicator.activeSelf != visible)
        {
            indicator.SetActive(visible);
        }
    }

    void ChangeCurrentSelectedUnit(GameObject selectedObject, int objectCost)
    {
        if (!selectedObject) return;
        UnitStatus status = selectedObject.GetComponent<CharacterBase>().Status;
        if (!status) return;

        selected = true;
        size = status.colliderRadius * 2.0f;
        transform.localScale = new Vector3(size, size, transform.localScale.z);
    }

    void DisablePreview()
    {
        gameObject.SetActive(false);
    }
}
