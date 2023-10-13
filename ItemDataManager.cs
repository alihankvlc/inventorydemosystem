using InventoryManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryManager
{
    public class ItemDataManager : Singleton<ItemDataManager>
    {
        [SerializeField] private ItemDatabase _database;
        private Dictionary<int, SCItem> _itemCache = new Dictionary<int, SCItem>();
        private InventoryManager _inventoryManager;
        void Awake()
        {
            if (_database == null)
                _database = Resources.Load<ItemDatabase>("ItemDatabase");

            _inventoryManager = InventoryManager.Instance;
        }
        private void Start()
        {
            InitializeDatabase();
        }
        private void InitializeDatabase()
        {
            foreach (SCItem item in _database.GetData)
            {
                _itemCache.Add(item.GetItemID, item);
            }
        }
        public SCItem GetItemByID(int id, int count)
        {
            if (_itemCache.TryGetValue(id, out SCItem item))
            {
                for (int i = 0; i < count; i++)
                {
                    _inventoryManager.AddItemToSlot(item, id);
                }
                return item;
            }
            else
            {
                Debug.Log("The item with the specified ID does not exist.");
                return null;
            }
        }
    }
}
