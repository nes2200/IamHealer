using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class UnitPlaceIndicator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float navMeshCheckRadius = 0.1f;
    [SerializeField] LayerMask floorLayer;

    [Header("Indicator")]
    [SerializeField] GameObject indicator;
    [SerializeField] Material indicatorMat;
    [SerializeField] DecalProjector decal;

    Camera mainCam;

    readonly int tintColorPorpertyID = Shader.PropertyToID("_TintColor");
    int floorLayerNum;

    float size;
    bool selected = false;

    bool _canSpawn;
    public bool CanSpawn => _canSpawn;

    private void OnEnable()
    {
        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;
        InputManager.OnUnitSelect += ChangeCurrentSelectedUnit;

        InputManager.OnMouseMove -= MoveToMouse;
        InputManager.OnMouseMove += MoveToMouse;

        StageManager.OnBattleStart -= DisablePreview;
        StageManager.OnBattleStart += DisablePreview;

        indicatorMat.SetColor(tintColorPorpertyID, Color.gray);
        floorLayerNum = LayerMask.NameToLayer("Floor");

        mainCam = Camera.main;
    }
    private void OnDisable()
    {
        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;
        InputManager.OnMouseMove -= MoveToMouse;
        StageManager.OnBattleStart -= DisablePreview;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 1000f, floorLayer))
        {
            SetIndicatorVisual(true);

            transform.position = hit.point + new Vector3 (0f, 5f, 0f);
            CheckSpawnable(hit.point);
        }
        else
        {
            SetIndicatorVisual(false);
        }
    }

    void CheckSpawnable(Vector3 floorPosition)
    {
        //선택된 유닛이 없다면 ,지금 마우스가 가리키는 오브젝트가 floor가 아니라면 생성 안함.
        if (!selected || GameManager.Instance.Input.IsMouseOverUI())
        {
            _canSpawn = false;
            return;
        }

        bool SpawnableCheck = NavMesh.SamplePosition(floorPosition, out NavMeshHit hit, navMeshCheckRadius, NavMesh.AllAreas);

        if(_canSpawn != SpawnableCheck)
        {
            if (SpawnableCheck)
            {
                Debug.Log("스폰 가능");
                indicatorMat.SetColor(tintColorPorpertyID, new Color(0f, 1f, 0f, 0.6f));
            }
            else
            {
                Debug.Log("불가능");
                indicatorMat.SetColor(tintColorPorpertyID, new Color(1f, 0f, 0f, 0.6f));
            }
            _canSpawn = SpawnableCheck;
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

        if (decal)
        {
            decal.size = new Vector3(size, size, decal.size.z);
        }
        _canSpawn = !NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshCheckRadius, NavMesh.AllAreas);
    }

    void DisablePreview()
    {
        gameObject.SetActive(false);
    }

}
