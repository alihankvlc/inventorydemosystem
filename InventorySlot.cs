using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

namespace InventoryManager
{
    public class InventorySlot : Singleton<InventorySlot>, IDropHandler
    {
        [field: SerializeField]
        public int GetSlotIndex { get; set; }
        [field: SerializeField]
        public int GetCountOfItemInSlot { get; set; }
        public void OnDrop(PointerEventData eventData)
        {
            if (transform.childCount == 0)
            {
                GameObject target = eventData.pointerDrag;
                DraggableItem targetDraggableItem = target.GetComponent<DraggableItem>();
                targetDraggableItem.GetParentAfterDrag = transform;
            }
            else
            {
                GameObject dropped = eventData.pointerDrag;
                DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

                GameObject current = transform.GetChild(0).gameObject;

                SCItem targetItem = current.GetComponent<Item>().GetItem;
                SCItem droppedItem = dropped.GetComponent<Item>().GetItem;

                DraggableItem currentDraggable = current.GetComponent<DraggableItem>();

                if (targetItem.GetItemID == droppedItem.GetItemID & targetItem.GetItemIsStackable)
                {
                    int dropObject = draggableItem.GetParentAfterDrag.GetComponent<InventorySlot>().GetCountOfItemInSlot;
                    if (dropObject + GetCountOfItemInSlot < InventoryManager.Instance.GetNumberOfStackableItems)
                    {
                        GetCountOfItemInSlot += dropObject;
                        TextMeshProUGUI slotCountTextMeshPro = InventoryManager.Instance.GetSlotList[GetSlotIndex].GetComponentInChildren<TextMeshProUGUI>();
                        slotCountTextMeshPro.SetText(GetCountOfItemInSlot.ToString());
                        draggableItem.GetPreviousSlot.GetCountOfItemInSlot = 0;
                        Destroy(dropped);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    draggableItem.GetPreviousSlot.GetComponent<InventorySlot>().GetCountOfItemInSlot = GetCountOfItemInSlot;
                    currentDraggable.transform.SetParent(draggableItem.GetParentAfterDrag);
                    draggableItem.GetParentAfterDrag = transform;
                    draggableItem.GetParentAfterDrag.GetComponent<InventorySlot>().GetCountOfItemInSlot = draggableItem.GetPreviousItemCount;
                }
            }
        }
    }
}

