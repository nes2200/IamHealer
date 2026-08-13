using System.Collections.Generic;
using UnityEngine;

public class StageDataAuthoring : MonoBehaviour
{
    [SerializeField] List<GameObject> selectableUnitsEntry = new();

    public IReadOnlyList<GameObject> SelectableUnitsEntry => selectableUnitsEntry;

    //청소 및 리셋용
    public void ClearSelectableUnits()
    {
        selectableUnitsEntry.Clear();
    }

    //로드할 때 내용 채워넣기
    public void SetSelectableUnits(IEnumerable<GameObject> unitPrefabs)
    {
        selectableUnitsEntry.Clear();

        if (unitPrefabs == null) return;

        foreach (GameObject unitPrefab in unitPrefabs)
        {
            if (!unitPrefab) continue;

            if (selectableUnitsEntry.Contains(unitPrefab)) continue;

            selectableUnitsEntry.Add(unitPrefab);   
        }
    }
}
