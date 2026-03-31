using UnityEngine;

[System.Serializable]
public struct PoolSetting
{
    public string poolName;
    public GameObject target;
    public uint countInitial; //처음에 준비할 개수
    public uint countAdditional; //부족하면 추가할 개수
}

[CreateAssetMenu(fileName = "PoolRequest", menuName = "PoolRequest/DefaultPoolRequest")]
public class PoolRequest : ScriptableObject
{
    public PoolSetting[] settings;
}
 