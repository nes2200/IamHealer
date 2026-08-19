using UnityEngine;
using System.Collections.Generic;

public class UI_HPBarGroup : MonoBehaviour
{
    [SerializeField] GameObject hpBarPrefab;
    Dictionary<CharacterBase, UI_HPBar> hpBars = new();
    StageCharacterRegistry connectedRegistry;

    public void Connect(StageCharacterRegistry registry)
    {
        if (connectedRegistry == registry) return;

        Disconnect();
        ClearAllHPBars();

        connectedRegistry = registry;
        if (!connectedRegistry) return;

        connectedRegistry.OnCharacterAdded -= CreateHPBar;
        connectedRegistry.OnCharacterAdded += CreateHPBar;

        connectedRegistry.OnCharacterRemoved -= RemoveHPBar;
        connectedRegistry.OnCharacterRemoved += RemoveHPBar;

        foreach(CharacterBase character in connectedRegistry.Characters)
        {
            CreateHPBar(character);
        }
    }
    
    public void Disconnect()
    {
        if (!connectedRegistry) return;

        connectedRegistry.OnCharacterAdded -= CreateHPBar;
        connectedRegistry.OnCharacterRemoved -= RemoveHPBar;
        connectedRegistry = null;
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
            if (!hpBar) continue;

            hpBar.Remove();
            ObjectManager.DestroyObject(hpBar.gameObject);
        }
        hpBars.Clear();
    }

}
