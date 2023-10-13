using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace InventoryManager
{
    [CreateAssetMenu(menuName = "Item/Create New Item")]
    public class SCItem : ScriptableObject
    {
        [SerializeField] private string _itemName;
        [SerializeField] private string _itemDescription;
        [SerializeField] private int _itemID;
        [SerializeField] private float _itemWeight;
        [SerializeField] private Sprite _itemIcon;
        [SerializeField] private bool _itemIsStackable;
        [SerializeField] private ItemType _itemType;
        public enum ItemType { None, Weapon, Consumable,Resource }
        public ItemType GetItemType { get => _itemType; }
        public int GetItemID { get => _itemID; }
        public float GetItemWeight { get => _itemWeight; }
        public string GetItemName { get => _itemName; }
        public string GetItemDescription { get => _itemDescription; }
        public Sprite GetItemIcon { get => _itemIcon; }
        public bool GetItemIsStackable { get => _itemIsStackable; }
    }
}
