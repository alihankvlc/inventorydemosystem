using InventoryManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryManager
{
    [CreateAssetMenu(menuName = "Item/Create Itemdatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<SCItem> _itemList;
        public List<SCItem> GetData { get => _itemList; }
    }
}
