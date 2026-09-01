using UnityEngine;

public enum UnitJob
{
    None,
    Knight,
    Swordsman,
    Archer,
    Mage,
    Fighter,
    Rogue,
    Warlock,
    Hunter,
    Sorcerer
}

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "Scriptable Objects/Unit Definition")]
public class UnitDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] UnitJob job;

    [Header("Shared Base")]
    [SerializeField] GameObject unitPrefab;

    [Header("Job Data")]
    [SerializeField] UnitStatus status;
    [SerializeField] RuntimeAnimatorController animatorController;

    public UnitJob Job => job;
    public GameObject UnitPrefab => unitPrefab;
    public UnitStatus Status => status;
    public RuntimeAnimatorController AnimatorController => animatorController;

    public bool IsValid => job != UnitJob.None && unitPrefab && status && animatorController;
}
