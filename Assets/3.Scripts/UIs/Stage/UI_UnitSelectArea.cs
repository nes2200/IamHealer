using System.Collections.Generic;
using UnityEngine;

public class UI_UnitSelectArea : MonoBehaviour
{
    [Header("Unit Button")]
    [SerializeField] UI_Button_UnitSelect buttonPrefab;
    [SerializeField] Transform buttonParent;

    [SerializeField] GameObject contentRoot;

    readonly List<UI_Button_UnitSelect> createdButtons = new();

    private void OnEnable()
    {
        if (GameManager.StageLoad == null) return;
        GameManager.StageLoad.OnSelectableUnitsLoaded -= RebuildButtons;
        GameManager.StageLoad.OnSelectableUnitsLoaded += RebuildButtons;
        RebuildButtons(GameManager.StageLoad.SelectableUnitDefinitions);
    }

    private void OnDisable()
    {
        GameManager.StageLoad.OnSelectableUnitsLoaded -= RebuildButtons;
    }

    public void RebuildButtons(IReadOnlyList<UnitDefinition> unitDefinitions)
    {
        ClearButtons();

        if (unitDefinitions == null) return;

        foreach(UnitDefinition unitDefinition in unitDefinitions)
        {
            if (!unitDefinition) continue;

            UI_Button_UnitSelect button = Instantiate(buttonPrefab, buttonParent);
            button.Initialize(unitDefinition);
            createdButtons.Add(button);
        }
    }

    private void ClearButtons()
    {
        foreach (UI_Button_UnitSelect button in createdButtons)
        {
            if (button)
            {
                Destroy(button.gameObject);
            }
        }
        createdButtons.Clear();
    }

    public void SetVisible(bool value)
    {
        contentRoot.SetActive(value);
    }
}
