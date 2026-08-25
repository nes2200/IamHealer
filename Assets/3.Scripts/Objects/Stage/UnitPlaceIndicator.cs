using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public enum UnitPlacementMode
{
    Place, Remove
}

public class UnitPlaceIndicator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float navMeshCheckRadius = 0.1f;
    [SerializeField] float heightOffset = 5f;
    [SerializeField] LayerMask floorLayer;
    [SerializeField] LayerMask unitLayer;
    [SerializeField] PlacementManager placementManager;

    [Header("Indicator")]
    [SerializeField] GameObject indicator;
    [SerializeField] Material indicatorMat;
    [SerializeField] DecalProjector decal;
    [SerializeField] BoxCollider indicatorCollider;
    [SerializeField] UnitPlacementMode currentMode = UnitPlacementMode.Place;

    //현재 유닛 설치모드인가 제거모드인가
    public UnitPlacementMode CurrentMode => currentMode;
    public bool IsRemoveMode => currentMode == UnitPlacementMode.Remove;

    Camera mainCam;

    readonly int tintColorPorpertyID = Shader.PropertyToID("_TintColor");

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

        StageManager.OnBattleStart -= DisableIndicator;
        StageManager.OnBattleStart += DisableIndicator;

        UI_StageScreen.OnMenuOpen -= UpdateIndicatorStatusByMenuOpen;
        UI_StageScreen.OnMenuOpen += UpdateIndicatorStatusByMenuOpen;

        UI_StageScreen.OnMenuClose -= UpdateIndicatorStatusByMenuClose;
        UI_StageScreen.OnMenuClose += UpdateIndicatorStatusByMenuClose;

        PlacementManager.OnUnitSpawn -= OnUnitSpawned;
        PlacementManager.OnUnitSpawn += OnUnitSpawned;

        PlacementManager.OnUnitDespawn -= OnUnitDespawned;
        PlacementManager.OnUnitDespawn += OnUnitDespawned;


        indicatorMat.SetColor(tintColorPorpertyID, Color.gray);

        mainCam = Camera.main;
    }
    private void OnDisable()
    {
        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;
        InputManager.OnMouseMove -= MoveToMouse;
        StageManager.OnBattleStart -= DisableIndicator;
        UI_StageScreen.OnMenuOpen -= UpdateIndicatorStatusByMenuOpen;
        UI_StageScreen.OnMenuClose -= UpdateIndicatorStatusByMenuClose;
        PlacementManager.OnUnitSpawn -= OnUnitSpawned;
        PlacementManager.OnUnitDespawn -= OnUnitDespawned;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        //게임이 멈춘 상태라면 갱신 안함
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        //UI위에 있으면 그냥 사라질거임
        if(GameManager.Input.IsMouseOverUI)
        {
            SetIndicatorActive(false);
            _canSpawn = false;
            return;
        }
        UpdateIndicatorStatus(screenPosition);
    }

    void UpdateIndicatorStatus(Vector2 screenPosition)
    {
        switch (currentMode)
        {
            case UnitPlacementMode.Place:
                UpdatePlaceIndicator(screenPosition);
                break;
            case UnitPlacementMode.Remove:
                UpdateRemoveIndicator(screenPosition);
                break;
        }
    }
    void UpdatePlaceIndicator(Vector2 screenPosition)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, floorLayer))
        {
            SetIndicatorActive(true);

            transform.position = hit.point + new Vector3(0f, heightOffset, 0f);
            indicatorCollider.center = new Vector3(0f, 0f, heightOffset);
            CheckSpawnable(hit.point);
        }
        else
        {
            SetIndicatorActive(false);
        }
    }
    void UpdateRemoveIndicator(Vector2 screenPosition)
    {
        GameObject target = InputManager.CursorHoverObject;

        if(!target || target.layer != LayerMask.NameToLayer("Character"))
        {
            SetIndicatorActive(false);
            return;
        }
        CharacterBase character = target.GetComponent<CharacterBase>();
        if (!character || character.Team == TeamID.TeamB)
        {
            SetIndicatorActive(false);
            return;
        }

        SetIndicatorActive(true);
        transform.position = character.transform.position + Vector3.up * heightOffset;

        UpdateIndicatorColor(false);
    }

    void CheckSpawnable(Vector3 floorPosition)
    {
        //선택된 유닛이 없다면 ,지금 마우스가 가리키는 오브젝트가 UI라면 생성 안함
        if (!selected)
        {
            _canSpawn = false;
            return;
        }

        bool spawnableCheck = NavMesh.SamplePosition(floorPosition, out NavMeshHit hit, navMeshCheckRadius, NavMesh.AllAreas);

        //마우스가 navMesh 위에는 있지만 적 진영이라 생성 불가할 경우
        //indicator의 중심은 마우스이기에 x 기준을 0으로 하면 유닛 절반이 상대 진영으로 넘어가도 소환 가능함
        //그렇기에 유닛의 size 보정을 줘서 넘어가지 않게 해주기
        if(spawnableCheck && hit.position.x > -size)
        {
            spawnableCheck = false;
        }
        //인디케이터와 유닛 충돌 체크
        if (spawnableCheck)
        {
            spawnableCheck = CheckUnitSpawnableOnCurrentLocation();
        }

        //이전 상태와 다를때 한 번만 색상이 바뀌게
        if(_canSpawn != spawnableCheck)
        {
            _canSpawn = spawnableCheck;
            UpdateIndicatorColor(_canSpawn);
        }
    }

    //인디케이터와 유닛이 닿았는지 체크
    public bool CheckUnitSpawnableOnCurrentLocation()
    {
        if (!indicatorCollider) return true; 

        //박스 콜라이더 월드 좌표 중심 계산
        Vector3 center = indicatorCollider.bounds.center;
        //박스콜라이더의 반경
        Vector3 halfExtents = indicatorCollider.bounds.extents;
        //박스의 회전값
        Quaternion orientation = indicatorCollider.transform.rotation;

        //영역 내 'unitLayer'를 가진 콜라이더가 하나라도 있는지 검사
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, orientation, unitLayer);

        return hitColliders.Length == 0;
    }

    //설치 제거 토글
    public void ToggleMode()
    {
        SetMode(CurrentMode == UnitPlacementMode.Place ? UnitPlacementMode.Remove : UnitPlacementMode.Place);
    }
    public void SetMode(UnitPlacementMode newMode)
    {
        currentMode = newMode;

        _canSpawn = false;
        RefreshIndicatorStatus();
    }

    //마우스 이동 업데이트가 멈췄을 경우 인디케이터 상태 강제 리프레시 해주기
    public void RefreshIndicatorStatus()
    {
        MoveToMouse(InputManager.CursorScreenPosition, InputManager.CursorWorldPosition);
    }

    public void OnUnitSpawned(CharacterBase _)
    {
        _canSpawn = false;
        UpdateIndicatorColor(false);
    }
    public void OnUnitDespawned(CharacterBase _)
    {
        StartCoroutine(CoRefreshAfterDespawn());
    }
    IEnumerator CoRefreshAfterDespawn()
    {
        //오브젝트 Destroy가 프레임 종료로 실제로 처리되기까지 대기
        yield return null;

        //transfomr과 collider 변경을 물리시스템에 적용
        Physics.SyncTransforms();
        RefreshIndicatorStatus();
    }

    public Vector3 GetCurrentIndicatorLoaction()
    {
        return transform.position - new Vector3(0f, heightOffset, 0f); 
    }

    void SetIndicatorActive(bool visible)
    {
        if (indicator.activeSelf != visible)
        {
            indicator.SetActive(visible);
        }
    }
    void UpdateIndicatorStatusByMenuOpen()
    {
        //일단은 빈칸
    }
    void UpdateIndicatorStatusByMenuClose()
    {
        UpdateIndicatorStatus(InputManager.CursorScreenPosition);
    }

    void UpdateIndicatorColor(bool canSpawn)
    {
        if (canSpawn)
        {
            indicatorMat.SetColor(tintColorPorpertyID, new Color(0f, 1f, 0f, 0.6f));
        }
        else
        {
            indicatorMat.SetColor(tintColorPorpertyID, new Color(1f, 0f, 0f, 0.6f));
        }
    }

    void ChangeCurrentSelectedUnit(GameObject selectedObject, int objectCost)
    {
        if (!selectedObject) return;
        UnitStatus status = selectedObject.GetComponent<CharacterBase>().Status;
        if (!status) return;

        SetMode(UnitPlacementMode.Place);
        selected = true;
        size = status.colliderRadius;

        if (decal)
        {
            decal.size = new Vector3(size * 2f, size * 2f, decal.size.z);
        }
        if (indicatorCollider)
        {
            indicatorCollider.size = new Vector3(size * 2f, size * 2f, indicatorCollider.size.z);
        }
    }

    //다시 로딩하지 않는 한 켜지지 않도록 꺼버리기(혹시나 몰라서, 나중에 필요하면 수정할꺼)
    void DisableIndicator()
    {
        gameObject.SetActive(false);
    }
}
