using InventoryManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryManager
{
    public class ItemDrop : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            InventorySlot slot = draggableItem.GetParentAfterDrag.GetComponent<InventorySlot>();

            int itemCount = slot.GetCountOfItemInSlot;
            int itemID = dropped.GetComponent<Item>().GetItem.GetItemID;

            InventoryManager.Instance.ItemDrop(dropped, itemID, itemCount);
            slot.GetCountOfItemInSlot = 0;
        }
    }
}
