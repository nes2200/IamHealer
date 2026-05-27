using System;
using UnityEngine;

public class ItemSlot
{
    //이 칸에 들어있는 아이템의 정보
    [SerializeField] ItemContainer item;
    //이 칸 만의 정보
    [SerializeField] int currentStack;

    public virtual bool Containable(ItemContainer newItem)
    {
        if (item)   return true;
        else        return false;
    }

    public ItemContainer GetItem()  => item;
    public int GetStack()           => currentStack;
    public bool GetIsMax()          => item ? currentStack >= item.maxStack : false;

    public int AddItem(ItemContainer wantItem, int amount)
    {
        //아이템이 없어
        if (wantItem is null) return 0;
        //준게 없어
        if (amount <= 0) return 0; 
        //준게 내가 가지고 있는거랑 달라
        if (item is not null && item != wantItem) return amount;

        return amount;
    }
}
