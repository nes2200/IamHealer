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
    public int GetStackable(ItemContainer wantItem) => Containable(wantItem) ? wantItem.maxStack - currentStack : 0;
    public int GetStackable()       => GetStackable(item);
    public int GetStack()           => currentStack;
    public int GetHalfStack()       => Mathf.CeilToInt(currentStack * 0.5f);
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

    public int GiveItem(ItemSlot wantSlot) => GiveItem(wantSlot, currentStack);
    public int GiveSingleItem(ItemSlot wantSlot) => GiveItem(wantSlot, 1);
    public int GiveHalfItem(ItemSlot wantSlot) => GiveItem(wantSlot, GetHalfStack());
    public int GiveItem(ItemSlot wantSlot, int amount)
    {
        if (wantSlot is null) return amount;
        if (!item) return amount;
        if (currentStack <= 0 || amount <= 0) return amount;

        ItemContainer targetItem = item;
        //원하는 개수는 대상의 절반, 혹은 가져올 수 있는 값 중 작은 값
        amount = Mathf.Min(amount, wantSlot.GetStackable(targetItem));
        //아이템 뺐는데 못뺀 만큼을 다시 돌려주니까
        //최종 얻은 amount의 개수에서 못 뺀 개수를 제한다
        amount -= RemoveItem(targetItem, amount);
        //빼온 만큼 다시 저장해서 돌려준다
        amount = wantSlot.AddItem(targetItem, amount);
        return amount;
    }

    public void LeftClick(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;

        if (InputManager.IsShift)
        {
            //대상에 아이템이 없는 경우
            if (wantSlot.GetIsEmpty())
            {
                //나도 없다
                if (GetIsEmpty()) return;
                //내 아이템을 받을 수 있을 경우
                else if (wantSlot.Containable(item))
                {
                    GiveItem(wantSlot, GetHalfStack());
                }
            }
            //대상에 아이템이 있고, 가져올 수 있는 경우
            else if (Containable(wantSlot.item))
            {
                //상대가 나에게 아이템의 절반을 준다
                wantSlot.GiveItem(this, GetHalfStack());
            }
        }
        else
        {
            if (wantSlot.Containable(item))
            {
                GiveItem(wantSlot);
            }
            else //클릭한 아이템이 들고있는 아이템과 다르다면
            {
                ExchangeItem(wantSlot);
            }
        }

        SlotChangeNotify();
        wantSlot.SlotChangeNotify();
    }
    public void RightClick(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;

        //대상이 비어있거나, shift를 누르고 있다면
        if(InputManager.IsShift || wantSlot.GetIsEmpty())
        {
            //하나를 주기
            if (wantSlot.Containable(item)) GiveSingleItem(wantSlot);
        }
        //가져올 수 있을 경우
        else if (Containable(wantSlot.item))
        {
            //하나 가져오기
            wantSlot.GiveSingleItem(this);
        }

        SlotChangeNotify();
        wantSlot.SlotChangeNotify();
    }
}
