using UnityEngine;

public enum EquipmentGrade
{
    Trash, Normal, Magic, Rare, Unique, Legendary,
    _Length
}


[CreateAssetMenu(fileName = "Item_Equipment", menuName = "Item/Equipment")]
public class Item_Equipment : ItemContainer
{
    public EquipmentGrade grade;

    public virtual void OnEquip(CharacterBase target)
    {

    }
    public virtual void OnUnequip(CharacterBase target)
    {

    }
}
