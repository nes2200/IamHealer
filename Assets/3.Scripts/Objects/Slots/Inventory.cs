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

    readonly string[] itemList = {"Axe", "Bread", "Crystallize Branch", "GreatPotion", "LesserPotion"};

    public void AxePluse(int amount)
    {
        int index = Random.Range(0, itemList.Length);
        ItemContainer axe = DataManager.LoadDataFile<ItemContainer>(itemList[index]);
        AddItem(axe, amount);
    }
    
    public void AxeRemove()
    {
        ItemContainer axe = DataManager.LoadDataFile<ItemContainer>("Axe");
        RemoveItem(axe);
    }

    //Comparison의 반환값 => int
    //음수 : 왼쪽이 작다
    //0   : 같다
    //양수 : 왼쪽이 크다
    public void Sort(System.Comparison<ItemSlot> Method)
    {
        MergeAll(); //정렬 시작 전 병합

        int totalLength = slots.Length;
        if (slots is null || totalLength <= 1) return;
        int width = slots.GetLength(1);

        int lastFinder = totalLength - 1;
        while (lastFinder > 0)
        {
            int currentFinder = -1;
            for (int i = 0; i < lastFinder; i++)
            {
                ItemSlot left = GetSlot(i, width);
                ItemSlot right = GetSlot(i + 1, width);
                int comparisonResult = Method(left, right);
                //if (comparisonResult > 0) //왼쪽이 더 클때 -> 작은놈을 왼쪽으로 가져오겠다 -> 오름차순으로 정렬
                if (comparisonResult < 0) //왼쪽이 더 작을때 -> 큰놈을 왼쪽으로 가져오겠다 -> 내림차순으로 정렬
                {
                    currentFinder = i;
                    left.ExchangeItem(right);
                }
            }
            lastFinder = currentFinder;
        }

        foreach(ItemSlot currentSlot in GetAllSlot())
        {
            currentSlot?.SlotChangeNotify();
        }
    }
    int ItemTypeComparison(ItemSlot left, ItemSlot right)
    {
        int result;
        if (ItemExistComparison(left, right, out result)) return result;

        ItemContainer leftItem = left.GetItem();
        ItemContainer rightItem = right.GetItem();

        result = leftItem.CompareByType(rightItem);
        if (result != 0) return result;
        result = left.GetStack() - right.GetStack();
        return result;
    }
    int? ItemExistComparison(ItemSlot left, ItemSlot right)
    {
        //왼쪽이 없다면
        if(left is null)
        {
            //거기에 오른쪽도 없다면
            if (right is null) return 0;
            //오른쪽이 있다면
            else return -1;
        }
        //여기 왔다면, 왼쪽은 있다는 것임. 그럼 오른쪽 유무만 체크하면 됨
        if (right is null) return 1;

        ItemContainer leftItem = left.GetItem();
        ItemContainer rightItem = right.GetItem();
        if (!leftItem)
        {
            if (!rightItem) return 0;
            else return -1;
        }
        if (!rightItem) return 1;

        //다 있으면 null 반환
        return null;
    }
    bool ItemExistComparison(ItemSlot left, ItemSlot right, out int result)
    {
        int? value = ItemExistComparison(left, right);
        result = value ?? 0;
        return value.HasValue; //값이 나왔으면 됬다
    }
    public void SortByType() => Sort(ItemTypeComparison);
    
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
        if (!wantItem) return 0;

        int result = 0;
        //해당하는 아이템을 가지고 있는 슬롯들을 모두 찾아와서
        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            //리스트에 넣어주기
            result += currentSlot.GetStack();
        }
        return result;
    }
    public int CountItem(ItemContainer wantItem, out List<ItemSlot> returnSlots)
    {
        returnSlots = new();
        if (!wantItem) return 0;

        int result = 0;
        //해당하는 아이템을 가지고 있는 슬롯들을 모두 찾아와서
        foreach(ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            //리스트에 넣어주기
            returnSlots.Add(currentSlot);
            result += currentSlot.GetStack();
        }
        return result;
    }

    public ItemSlot GetSlot(int index)
    {
        if (slots is null || slots.Length == 0 || slots.Length <= index || index < 0) return null ;
        int width = slots.GetLength(1);
        return slots[index / width, index % width];
    }
    public ItemSlot GetSlot(int index, int width) => slots[index / width, index % width];
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

    public IEnumerable<ItemContainer> GetAllItem()
    {
        //List는 추가나 제거가 쉬운 대신 순서가 있다
        //무언가를 찾으려면 처음부터 돌아야 하기 때문에 손해가 있음
        //Set => 검색에 용이하도록 만들어진 자료구조 => 중복 허용 안하고 순서 상관 없음
        //SortedSet : 값의 크기에 따라 저장
        //HashedSet : 값을 해시로 변경해서 저장
        //해시 => 자료를 변경하여 동일한 길이의 숫자로 바꿈
        //되돌릴 수 없는, 그렇지만 다시 같은 경우를 반복할 수 있는 함수

        HashSet<ItemContainer> usedItem = new();
        foreach(ItemSlot currentSlot in GetAllSlot())
        {
            ItemContainer currentItem = currentSlot.GetItem();
            if (!currentItem) continue;
            //추가가 안된다 -> 이미 있다
            if (!usedItem.Add(currentItem)) continue;
            yield return currentItem;
        }
    }
    public Dictionary<ItemContainer, List<ItemSlot>> GetAllItemList()
    {
        Dictionary<ItemContainer, List<ItemSlot>> result = new();

        foreach (ItemSlot currentSlot in GetAllSlot())
        {
            ItemContainer currentItem = currentSlot.GetItem();
            if (!currentItem) continue;
            //이미 딕셔너리에 해당 아이템이 존재하는 경우
            if(result.TryGetValue(currentItem, out List<ItemSlot> currentList))
            {
                //리스트에 포함시킨다
                currentList.Add(currentSlot);
            }
            else
            {
                result.Add(currentItem, new() { currentSlot });
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

    public void MergeAll()
    {
        foreach(ItemContainer currentItem in GetAllItem())
        {
            MergeItem(currentItem);
        }
    }
    public void MergeItem(ItemContainer wantItem)
    {
        if (!wantItem) return;
        int maxStack = wantItem.maxStack;
        if (maxStack <= 1) return;
        int totalCount = CountItem(wantItem, out List<ItemSlot> containSlots);
        if (totalCount <= 1) return;
        if (containSlots is null) return;
        int slotCount = containSlots.Count;
        if (totalCount >= slotCount * maxStack || slotCount <= 1) return;

        //모든 슬롯을 돌아주지만 마지막은 돌 필요가 없다
        int finalSlot = slotCount - 1;
        for(int i = 0; i < finalSlot; i++)
        {
            ItemSlot currentslot = containSlots[i];
            for(int j = finalSlot; j > i; j--)
            {
                //가득찬 슬롯은 병합할 필요 없음 -> 넘어가
                if (currentslot.GetIsMax()) break;

                ItemSlot targetSlot = containSlots[j];
                targetSlot.GiveItem(currentslot);
                //대상 슬롯이 비었으니 마지막 슬롯 체크를 안해도 됨
                if (targetSlot.GetIsEmpty()) finalSlot--;
            }
        }
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