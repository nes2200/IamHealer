using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
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

    public void AxePluse()
    {
        ItemContainer axe = DataManager.LoadDataFile<ItemContainer>("Axe");
        AddItem(axe);
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
    public ItemSlot[] GetAllSlot()
    {
        ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        Debug.Log($"높이 : {height}");
        Debug.Log($"길이 : {width}");

        for(int r = 0; r < height; r++)
        {
            for(int c = 0; c < width; c++)
            {
                result[width * r + c] = slots[r,c];
            }
        }
        return result;
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
    public ItemSlot FindFirstEmptySlot()
    {
        return default;
    }
    public ItemSlot FindLastEmptySlot()
    {
        return default;
    }
    public ItemSlot FindFirstItem(ItemContainer target)
    {
        return default;
    }
    public ItemSlot FindLastItem(ItemContainer target)
    {
        return default;
    }
    //바닥에 999개 있는데, 7개 주웠으면 몇개가 남아야 할까?
    //그 추가하지 못한 개수를 리턴할 것
    public int AddItem(ItemContainer wantItem, int amount = 1)
    {
        slots[0, 0].AddItem(wantItem, amount);
        return default;
    }
    public int AddItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        return default;
    }
    public int AddItemOnEmptySlots(ItemContainer wantItem, int amount)
    {
        return default;
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
    public int RemoveItem(ItemContainer wantItem)
    {
        return default;
    }
    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        return default;
    }
    public int RemoveItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        return default;
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
    public bool UseItem(ItemContainer target)
    {
        return default;
    }
    public bool UseItem(int row, int column)
    {
        return default;
    }
}