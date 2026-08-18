using System;
using System.Collections.Generic;
using UnityEngine;

public class StageCharacterRegistry : MonoBehaviour
{
    public event Action<CharacterBase> OnCharacterAdded;
    public event Action<CharacterBase> OnCharacterRemoved;

    readonly List<CharacterBase> characters = new();
    public IReadOnlyList<CharacterBase> Characters => characters;

    public void Register(CharacterBase character)
    {
        if (!character || characters.Contains(character)) return;

        characters.Add(character);
        OnCharacterAdded?.Invoke(character);
    }
    public void Unregister(CharacterBase character)
    {
        if (!character || !characters.Remove(character)) return;

        OnCharacterRemoved?.Invoke(character);
    }

    public void Clear()
    {
        for (int i = characters.Count - 1; i >= 0; i--)
            Unregister(characters[i]);
    }
}
