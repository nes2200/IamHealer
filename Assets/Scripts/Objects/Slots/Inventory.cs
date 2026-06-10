using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static ItemSlot cursorSlot = new ItemSlot();

    //가로 세로
    //columns rows
    public int columns;
    public int rows;

    //아이템 슬롯을 columns와 rows 개수만큼 준비해야 한다
    //2차원 행렬이 필요
    //대상을 여러개 저장, 개수가 바뀌지 않고, 순환이 빨라야 한다
    //배열
    ItemSlot[,] slots;

    public void Initialize()
    {
        //행, 열 순서로 움직이는 형태가 많다
        slots = new ItemSlot[rows, columns];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                slots[r, c] = new ItemSlot();
            }
        }
    }

    public void AxePluse(int amount)
    {
        ItemContainer axe = DataManager.LoadDataFile<ItemContainer>("Axe");
        AddItem(axe, amount);
    }
    public void AxeMinus(int amount)
    {
        ItemContainer axe = DataManager.LoadDataFile<ItemContainer>("Axe");
        RemoveItem(axe, amount);
    }
    public void AxeRemove()
    {
        ItemContainer axe = DataManager.LoadDataFile<ItemContainer>("Axe");
        RemoveItem(axe);
    }
    public void Sort(System.Comparison<ItemContainer> Method)
    {

    }

    public void AutoQuickInsert(Inventory other)
    {

    }
    public void AutoQuickInsert(Inventory[] other)
    {

    }

    public bool InsertAll(Inventory other)
    {
        return default;
    }
    public bool InsertAll(Inventory otehr, ItemContainer target)
    {
        return default;
    }

    public void LockSlot(int wantRow, int wantColumn)
    {

    }
    public void UnlockSlot(int wantRow, int wantColumn)
    {
    }

    public int CountItem(ItemContainer wantItem)
    {
        return default;
    }
    public int CountItem(ItemContainer wantItem, out List<ItemSlot> returnSlots)
    {
        returnSlots = default;
        return default;
    }

    //       반복기 => 원하는 자료형을 반복적으로 내보냄
    //                 요구할 때 마다 하나씩 나옴
    //                 ItemSlot을 요구할 떄 마다 다음 슬롯을 내놓는 기능
    public IEnumerable<ItemSlot> GetAllSlot()
    {
        //ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        for(int r = 0; r < height; r++)
        {
            for(int c = 0; c < width; c++)
            {
                if (slots[r, c] is null) continue;
                //yield return : 결과를 내보내고 나서 기다리기
                yield return slots[r,c];
            }
        }
    }
    public IEnumerable<ItemSlot> GetAllSlotReverse()
    {
        //ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        for (int r = height - 1; r >= 0; r--)
        {
            for (int c = width - 1; c >= 0; c--)
            {
                if (slots[r, c] is null) continue;
                yield return slots[r, c];
            }
        }
    }

    public ItemSlot FindItem(ItemContainer target)
    {
        return default;
    }
    public ItemSlot FindItem(ItemType wantType)
    {
        return default;
    }
    public ItemSlot FindItem(string containWord)
    {
        return default;
    }
    public ItemSlot FindItem(int wantRow, int wantColumn)
    {
        if (wantRow < 0 || wantColumn < 0)      return null;
        if (wantRow     >= slots.GetLength(0))  return null;
        if (wantColumn  >= slots.GetLength(1))  return null;

        return slots[wantRow, wantColumn];
    }

    //제일 왼쪽 위 첫 번째 슬롯을 찾고 싶다
    //찾은 다음에 그 뒤부터 다시 진행할 수 있는 방법
    //함수를 잠깐 멈춰놓았다가 나중에 또 부탁하는 방법
    //반복을 나중에 추가로 도는 방법
    public IEnumerable<ItemSlot> FindFirstEmptySlot()
    {
        foreach(ItemSlot currentSlot in GetAllSlot())
        {
            if (currentSlot.GetIsEmpty()) yield return currentSlot;
        }
    }
    public IEnumerable<ItemSlot> FindLastEmptySlot()
    {
        foreach (ItemSlot currentSlot in GetAllSlotReverse())
        {
            if (currentSlot.GetIsEmpty()) yield return currentSlot;
        }
    }
    public IEnumerable<ItemSlot> FindFirstItem(ItemContainer target)
    {
        foreach (ItemSlot currentSlot in GetAllSlot())
        {
            if (currentSlot.GetItem() == target) yield return currentSlot;
        }
    }
    public IEnumerable<ItemSlot> FindLastItem(ItemContainer target)
    {
        foreach (ItemSlot currentSlot in GetAllSlotReverse())
        {
            if (currentSlot.GetItem() == target) yield return currentSlot;
        }
    }

    //바닥에 999개 있는데, 7개 주웠으면 몇개가 남아야 할까?
    //그 추가하지 못한 개수를 리턴할 것
    public int AddItem(ItemContainer wantItem, int amount = 1)
    {
        //이미 같은 아이템이 존재하는 슬롯에 넣기
        amount = AddItemOnExistSlots(wantItem, amount);
        //넣어보니 남은게 없으면 끝
        if (amount <= 0) return 0;
        //그래도 남았으면 빈칸에 넣기
        return AddItemOnEmptySlots(wantItem, amount);
    }
    public int AddItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            if (amount <= 0) return 0;

            amount = currentSlot.AddItem(wantItem, amount);
            currentSlot.SlotChangeNotify();
        }
        return amount;
    }
    public int AddItemOnEmptySlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindFirstEmptySlot())
        {
            if (amount <= 0) return 0;

            amount = currentSlot.AddItem(wantItem, amount);
            currentSlot.SlotChangeNotify();
        }
        return amount;
    }
    public int AddItemToLocation(ItemContainer wantItem, int amount, int row, int column)
    {
        return default;
    }

    public ItemSlot[,] Clear()
    {
        ItemSlot[,] origin = slots;
        Initialize();
        return origin;
    }

    public int RemoveItem(System.Predicate<ItemContainer> condition)
    {
        return default;
    }
    //몇개 지워라 -> 몇개 못지웠는지 반환
    //다 지워라 -> 몇개 지웠는지 반환
    public int RemoveItem(ItemContainer wantItem)
    {
        int result = 0;
        foreach(ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            result = currentSlot.RemoveItem(wantItem);
            currentSlot.SlotChangeNotify();
        }
        return result;
    }
    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        //존재하는 칸에서 제거하기
        amount = RemoveItemOnExistSlots(wantItem, amount);
        //남은게 0 이하라면 끝
        if (amount <= 0) return 0;
        //존재하는 칸에서 제거하고 남은만큼을 반환하기
        return amount;
    }
    public int RemoveItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindLastItem(wantItem))
        {
            if (amount <= 0) return 0;

            amount = currentSlot.RemoveItem(wantItem, amount);
            currentSlot.SlotChangeNotify();
        }
        return amount;
    }
    public int RemoveItemFromLocation(int row, int column)
    {
        return default;
    }
    public int RemoveItemFromLocation(int row, int column, int amount)
    {
        return default;
    }
    public void MoveItem(int startRow, int startColumn, Inventory targetInventory,int targetRow, int targetColumn, int amount = -1)
    {
       
    }
    public void ExchangeItem(int startRow, int startColumn, ItemSlot targetSlot)
    {
        if (targetSlot is null) return;

        ItemSlot first = FindItem(startRow, startColumn);
        if (first is null) return;

        first.ExchangeItem(targetSlot); 
        first.SlotChangeNotify();
        targetSlot.SlotChangeNotify();
    }
    public void ExchangeItem(int startRow, int startColumn, int targetRow, int targetColumn)
    {
        ExchangeItem(startRow, startColumn, this, targetRow, targetColumn);
    }
    public void ExchangeItem(int startRow, int startColumn, Inventory targetInventory, int targetRow, int targetColumn)
    {
        ItemSlot first = FindItem(startRow, startColumn);
        if (first is null) return;

        if (!targetInventory) return;
        ItemSlot second = targetInventory.FindItem(targetRow, targetColumn);
        if (second is null) return;

        first.ExchangeItem(second);
        first.SlotChangeNotify();
        second.SlotChangeNotify();
    }
    public bool UseItem(ItemContainer target)
    {
        return default;
    }
    public bool UseItem(int row, int column)
    {
        return default;
    }
}