using UnityEngine;
using System.Collections.Generic;

public class UI_HPBarGroup : MonoBehaviour
{
    [SerializeField] GameObject hpBarPrefab;
    Dictionary<CharacterBase, UI_HPBar> hpBars = new();

    private void OnEnable()
    {
        PlacementManager.OnUnitSpawn -= CreateHPBar;
        PlacementManager.OnUnitSpawn += CreateHPBar;

        PlacementManager.OnUnitDespawn -= RemoveHPBar;
        PlacementManager.OnUnitDespawn += RemoveHPBar;
    }
    private void OnDisable()
    {
        PlacementManager.OnUnitSpawn -= CreateHPBar;
        PlacementManager.OnUnitDespawn -= RemoveHPBar;
    }

    public void CreateHPBar(CharacterBase connectedCharacter)
    {
        if (!connectedCharacter || hpBars.ContainsKey(connectedCharacter)) return;

        GameObject createdHPBar = ObjectManager.CreateObject(hpBarPrefab, transform);

        if (!createdHPBar.TryGetComponent(out UI_HPBar hpBar)) return;

        hpBar.Initialize(connectedCharacter);
        hpBars.Add(connectedCharacter, hpBar);
    }
    public void RemoveHPBar(CharacterBase connectedCharacter)
    {
        if (!hpBars.TryGetValue(connectedCharacter, out UI_HPBar hpBar)) return;
        hpBars.Remove(connectedCharacter);
        hpBar.Remove();
        ObjectManager.DestroyObject(hpBar.gameObject);
    }
    public void ClearAllHPBars()
    {
        foreach(UI_HPBar hpBar in hpBars.Values)
        {
            if (!hpBar) return;

            hpBar.Remove();
            ObjectManager.DestroyObject(hpBar.gameObject);
        }
        hpBars.Clear();
    }

}
