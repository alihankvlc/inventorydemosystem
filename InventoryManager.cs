using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using SurvivalProject;
using Unity.VisualScripting;

namespace InventoryManager
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private List<GameObject> _slotUIList;
        [SerializeField] private GameObject _itemReferanceObj;
        [SerializeField] private GameObject _itemDropPouchObject;
        [SerializeField] private Vector3 _itemDropOffset;
        [SerializeField] private int _numberOfStackableItems;
        [SerializeField] private float _maxWeight;
        [SerializeField] private List<Button> _toolBeltButtonList = new List<Button>();
        [SerializeField] private List<InventorySlot> _slotList = new List<InventorySlot>();
        [SerializeField] private List<InventorySlot> _toolbeltSlotList = new List<InventorySlot>();
        [SerializeField] private bool _inventoryIsFull;

        private Transform _playerTransform = null;
        private InputManager _inputManager;

        private int _previousToolBeltSlotIndex;
        private int _toolBeltSlotIndex = 0;
        private int _inventroySlotItemCount;
        private float _inventoryWeight;
        private float _lastConsumableUseTime;
        private const float CONSUMABLE_USE_INTERVAL = 1.5f;
        private const int TOOL_BELT_MAX_SLOT = 9;
        public List<InventorySlot> GetSlotList { get => _slotList; }
        public bool GetInventoryStatus { get => _inventoryIsFull; }
        public int GetNumberOfStackableItems { get => _numberOfStackableItems; }
        private void Awake()
        {
            _inputManager = new InputManager();

            CreateSlot();
            CreateToolBeltSlot();
            InitializeToolBeltButton();
        }
        private void OnEnable()
        {
            _inputManager.Enable();
            _inputManager.Player.ScrollNavigate.performed += NavigateToolBeltScroll;
            _inputManager.Player.KeyNavigate.performed += NavigateToolBeltKey;
        }
        private void Update()
        {
            EquipToToolBelt();
        }
        private void Start()
        {
            _playerTransform = FindAnyObjectByType<PlayerController>().transform;
        }
        #region TOOL_BELT_INVENTORY
        private void CreateToolBeltSlot()
        {
            for (int i = 0; i < TOOL_BELT_MAX_SLOT; i++)
                _toolbeltSlotList.Add(_slotList[i]);
        }
        private void NavigateToolBeltScroll(InputAction.CallbackContext context)
        {
            _previousToolBeltSlotIndex = _toolBeltSlotIndex;
            float scrollDelta = context.ReadValue<float>();
            if (scrollDelta > 0)
            {
                _toolBeltSlotIndex = (_toolBeltSlotIndex + 1) % TOOL_BELT_MAX_SLOT;
            }
            else if (scrollDelta < 0)
            {
                _toolBeltSlotIndex = (_toolBeltSlotIndex - 1 + TOOL_BELT_MAX_SLOT) % TOOL_BELT_MAX_SLOT;
            }

            if (_previousToolBeltSlotIndex != _toolBeltSlotIndex)
            {
                SetToolBeltSlotScale(_previousToolBeltSlotIndex, 1.0f, Color.green);
            }

            SetToolBeltSlotScale(_toolBeltSlotIndex, 1.1f, Color.green);
        }
        private void NavigateToolBeltKey(InputAction.CallbackContext context)
        {
            _previousToolBeltSlotIndex = _toolBeltSlotIndex;
            for (int i = 1; i <= TOOL_BELT_MAX_SLOT; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i - 1))
                {
                    _toolBeltSlotIndex = i - 1;
                }
            }
            if (_previousToolBeltSlotIndex != _toolBeltSlotIndex)
            {
                SetToolBeltSlotScale(_previousToolBeltSlotIndex, 1.0f, Color.green);
            }
            SetToolBeltSlotScale(_toolBeltSlotIndex, 1.1f, Color.green);
        }
        private void InitializeToolBeltButton()
        {
            for (int i = 0; i < _toolBeltButtonList.Count; i++)
            {
                int buttonIndex = i;
                _toolBeltButtonList[i].onClick.AddListener(() => OnToolBeltButtonClick(buttonIndex));
            }
        }
        private void OnToolBeltButtonClick(int buttonIndex)
        {
            SetToolBeltSlotScale(_toolBeltSlotIndex, 1.0f, Color.green);

            _toolBeltSlotIndex = buttonIndex;
            SetToolBeltSlotScale(_toolBeltSlotIndex, 1.1f, Color.green);
        }
        private void SetToolBeltSlotScale(int slotIndex, float scale, Color color)
        {
            if (slotIndex >= 0 && slotIndex < TOOL_BELT_MAX_SLOT)
            {

                _slotList[slotIndex].transform.localScale = Vector3.one * scale;
                color.a = 0.85f;
                _slotList[slotIndex].GetComponent<Image>().color = color;
                for (int i = 0; i < TOOL_BELT_MAX_SLOT; i++)
                {
                    if (i != slotIndex)
                    {
                        _slotList[i].transform.localScale = Vector3.one;
                        _slotList[i].GetComponent<Image>().color = Color.white;
                    }
                }
            }
        }
        public bool IsConsumableAvailable() => Time.time - _lastConsumableUseTime >= CONSUMABLE_USE_INTERVAL;
        private void EquipToToolBelt()
        {
            int index = _toolbeltSlotList[_toolBeltSlotIndex].GetSlotIndex;
            if (_toolbeltSlotList[index].transform.childCount == 0) { return; }
            else
            {
                SCItem item = _toolbeltSlotList[index]?.GetComponentInChildren<Item>().GetItem;
                GameObject itemObj = _toolbeltSlotList[index]?.GetComponentInChildren<Item>().transform.gameObject;
                if (item != null)
                {
                    switch (item.GetItemType)
                    {
                        case SCItem.ItemType.None:
                            break;
                        case SCItem.ItemType.Weapon:
                            Debug.Log("Silah kuþanýldý");
                            break;
                        case SCItem.ItemType.Consumable:
                            if (IsConsumableAvailable() & Input.GetButtonDown("Fire1"))
                            {
                                if (_toolbeltSlotList[index].GetCountOfItemInSlot > 1)
                                {
                                    _slotList[index].GetComponent<InventorySlot>().GetCountOfItemInSlot--;
                                    GetItemCountText(_toolbeltSlotList[index], _slotList[index].GetComponent<InventorySlot>().GetCountOfItemInSlot);
                                }
                                else
                                {
                                    Destroy(itemObj);
                                }
                                _lastConsumableUseTime = Time.time;
                            }
                            break;
                        case SCItem.ItemType.Resource:
                            break;
                    }
                }
            }
        }
        #endregion
        private void CreateSlot()
        {
            foreach (GameObject inventorySlotOBJ in _slotUIList)
            {
                if (inventorySlotOBJ.TryGetComponent(out InventorySlot slot))
                {
                    slot.GetSlotIndex = _slotList.Count;
                    _slotList.Add(slot);
                }
            }
        }
        private InventorySlot FindEmptySlot()
        {
            foreach (GameObject emptySlot in _slotUIList)
            {
                if (emptySlot.TryGetComponent(out InventorySlot slot))
                {
                    if (slot.transform.childCount == 0)
                    {
                        _inventoryIsFull = false;
                        return slot;
                    }
                }
            }
            _inventoryIsFull = true;
            return null;
        }
        private InventorySlot FindItemWithID(int itemID)
        {
            foreach (GameObject emptySlot in _slotUIList)
            {
                if (emptySlot.TryGetComponent(out InventorySlot slot))
                {
                    if (slot.transform.childCount > 0)
                    {
                        SCItem item = slot.GetComponentInChildren<Item>().GetItem;
                        if (item.GetItemID == itemID & slot.GetCountOfItemInSlot < _numberOfStackableItems)
                        {
                            return slot;
                        }
                    }
                }
            }
            return null;
        }
        private GameObject CreateEmptyItemChild(InventorySlot inventorySlot, string itemName)
        {
            if (inventorySlot.transform.childCount == 0 && _itemReferanceObj != null)
            {
                GameObject item = Instantiate(_itemReferanceObj, inventorySlot.transform.position, Quaternion.identity);
                item.name = itemName;
                item.transform.SetParent(inventorySlot.transform);
                return item;
            }
            return null;
        }
        private TextMeshProUGUI GetItemCountText(InventorySlot inventorySlot, int itemCount)
        {
            TextMeshProUGUI itemCounTextMeshPro = inventorySlot.GetComponentInChildren<TextMeshProUGUI>();
            itemCounTextMeshPro.SetText(itemCount.ToString());

            return itemCounTextMeshPro;
        }

        private Image GetItemImage(InventorySlot inventorySlot, Sprite itemIcon)
        {
            Image image = inventorySlot.transform.GetChild(0).GetComponent<Image>();
            image.sprite = itemIcon;
            return image;
        }

        internal void AddItemToSlot(SCItem item, int itemID)
        {
            InventorySlot existingSlot = FindItemWithID(itemID);

            if (existingSlot != null & item.GetItemIsStackable)
            {
                existingSlot.GetCountOfItemInSlot++;
                GetItemCountText(existingSlot, existingSlot.GetCountOfItemInSlot);
            }
            else
            {
                InventorySlot emptyInventorySlot = FindEmptySlot();

                if (emptyInventorySlot != null)
                {
                    GameObject slotItemGameObject = CreateEmptyItemChild(emptyInventorySlot, item.GetItemName);
                    SCItem slotItem = slotItemGameObject.GetComponent<Item>().GetItem = item;

                    _inventoryWeight += slotItem.GetItemWeight;
                    emptyInventorySlot.GetCountOfItemInSlot = 1;
                    GetItemImage(emptyInventorySlot, slotItem.GetItemIcon);
                    GetItemCountText(emptyInventorySlot, emptyInventorySlot.GetCountOfItemInSlot);
                }
            }
        }
        internal void ItemDrop(GameObject itemObject, int itemID, int itemCount)
        {
            GameObject pouch = Instantiate(_itemDropPouchObject, _playerTransform.position + _itemDropOffset, Quaternion.identity);
            DroppedItem droppedItem = pouch.GetComponent<DroppedItem>();

            pouch.transform.name = itemObject.name;
            droppedItem.GetDroppedItemID = itemID;
            droppedItem.GetDroppedItemCount = itemCount;

            FindEmptySlot();
            Destroy(itemObject);
        }
        private void OnDisable()
        {
            _inputManager.Disable();
            _inputManager.Player.ScrollNavigate.performed -= NavigateToolBeltScroll;
            _inputManager.Player.KeyNavigate.performed -= NavigateToolBeltKey;
        }
    }
}