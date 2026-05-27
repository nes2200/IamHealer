using System;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryWindow : OpenableUIBase
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] LayoutGroup layout;
    [SerializeField] string itemSlotPrefabName;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        targetInventory?.Initialize();
        ConnectInventory(targetInventory);
    }
    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        DisconnectInventory();
    }

    private void ConnectInventory(Inventory newInventory)
    {
        if (!newInventory) return;
        targetInventory = newInventory;

        if (!layout) return;

        if(layout is GridLayoutGroup asGridLayout)
        {
            asGridLayout.constraintCount = targetInventory.columns;
        }

        ItemSlot[] inventorySlots = newInventory.GetAllSlot();
        foreach(ItemSlot currentSlot in inventorySlots)
        {
            if (currentSlot is null) continue;

            GameObject instance = ObjectManager.CreateObject(itemSlotPrefabName, layout.transform);
            if (!instance) continue;
            if (instance.TryGetComponent(out UI_ItemSlotInfo createdSlot))
            {
                createdSlot.ConnectSlot(currentSlot);
            } 
            
        }
    }
    private void DisconnectInventory()
    {
        if (!layout) return;

        //³²Àº ÀÚ½Ä ÁË´Ù Á×ÀÌ±â
        while(layout.transform.childCount > 0)
        {
            Transform targetChild = layout.transform.GetChild(0);
            targetChild.SetParent(null);
            ObjectManager.DestroyObject(targetChild.gameObject);
        }
    }
}
