using System;
using UnityEngine;

public class UnitPlaceIndicator : MonoBehaviour
{
    [SerializeField] float floorOffset = 0.05f;
    [SerializeField] GameObject indicator;

    float size;
    bool selected = false;
    bool canSpawn;

    private void OnEnable()
    {
        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;
        InputManager.OnUnitSelect += ChangeCurrentSelectedUnit;

        InputManager.OnMouseMoveFloor -= MoveToMouse;
        InputManager.OnMouseMoveFloor += MoveToMouse;

        StageManager.OnBattleStart -= DisablePreview;
        StageManager.OnBattleStart += DisablePreview;

        //처음에는 선택된 유닛이 없으니까
        SetIndicatorVisual(false);

    }
    private void OnDisable()
    {
        InputManager.OnUnitSelect -= ChangeCurrentSelectedUnit;
        InputManager.OnMouseMoveFloor -= MoveToMouse;
        StageManager.OnBattleStart -= DisablePreview;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!selected) return;

        if (worldPosition.y < -9000f)
        {
            SetIndicatorVisual(false);
            return;
        }

        SetIndicatorVisual(true);
        transform.position = worldPosition + new Vector3(0f, floorOffset, 0f);
    }

    void SetIndicatorVisual(bool visible)
    {
        if(indicator.activeSelf != visible)
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
        indicator.transform.localScale = new Vector3(size, size, indicator.transform.localScale.z);
    }

    void DisablePreview()
    {
        gameObject.SetActive(false);
    }
}
