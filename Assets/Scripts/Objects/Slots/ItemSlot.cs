using Unity.VisualScripting;
using UnityEngine;

//아이템 슬롯이 바꼈어
public delegate void ItemSlotChangeEvent(ItemSlot changedSlot);

public class ItemSlot
{
    //이 칸에 들어있는 아이템의 정보
    [SerializeField] ItemContainer item;
    //이 칸 만의 정보
    [SerializeField] int currentStack;

    public event ItemSlotChangeEvent OnItemSlotChanged;

    public void SlotChangeNotify() => OnItemSlotChanged?.Invoke(this);

    public virtual bool Containable(ItemContainer wantItem)
    {
        //아이템이 없어
        if (wantItem is null)         return false;
        //준게 내가 가지고 있는거랑 달라
        if (item && item != wantItem) return false;
        //최대치야
        if (GetIsMax())               return false;

        return true;
    } 

    public ItemContainer GetItem()  => item;
    public int GetStack()           => currentStack;
    public bool GetIsMax()          => item ? currentStack >= item.maxStack : false;
    public bool GetIsEmpty()        => item == null || currentStack <= 0;

    public int Clear()
    {
        item = null;
        int removed = currentStack;
        currentStack = 0;
        return removed;
    }

    public int AddItem(ItemContainer wantItem, int amount)
    {
        if (amount <= 0) return 0;
        if (!Containable(wantItem)) return amount;

        item = wantItem;

        //넣을 수 있는 만큼만 넣기
        //최대값, 현재값, 추가할값을 비교하며 얼마나 넣어야 할지, 얼마나 돌려줘야 할지 계산해야 한다
        //최대값 - 현재값 = 추가 가능한 값
        //추가 가능한 값과 추가하고 싶은 값을 비교하여 적은것이 추가할 값
        int stackable = Mathf.Min(item.maxStack - currentStack, amount);
        currentStack += stackable;
        return amount - stackable; //추가하려는 값 - 추가한 값
    }
    //개수를 지정해주지 않았을 경우 : 몇개나 지웠는가를 반환
    public int RemoveItem(ItemContainer wantItem)
    {
        
        if (!wantItem) return 0;
        if (GetIsEmpty()) return 0;
        if (item != wantItem) return 0;

        return Clear();
    }
    //개수를 지정했을 경우 : 지울게 몇개 더 남았나를 반환
    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        if (amount <= 0) return 0;
        if (!wantItem) return 0;
        if (GetIsEmpty()) return amount;
        if (item != wantItem) return amount;

        if (amount >= currentStack)
        {
            return amount - Clear();
        }
        currentStack -= amount;
        return 0;
    }

    public void ExchangeItem(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;

        ItemContainer wasItem = item;
        int wasStack = currentStack;

        item = wantSlot.item;
        currentStack = wantSlot.currentStack;

        wantSlot.item = wasItem;
        wantSlot.currentStack = wasStack;
    }

    public void LeftClick(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;

        ExchangeItem(wantSlot);
        SlotChangeNotify();
        wantSlot.SlotChangeNotify();
    }
    public void RightClick()
    {

    }
}
