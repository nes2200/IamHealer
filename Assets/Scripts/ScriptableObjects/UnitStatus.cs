using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatus", menuName = "Scriptable Objects/UnitStatus")]
public class UnitStatus : ScriptableObject
{
    public string unitName;
    public int cost;
    public int maxHP;
    public int damage;
    public float attackSpeed;
    public float moveSpeed;
}
