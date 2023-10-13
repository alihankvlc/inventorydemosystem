using InventoryManager;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace InventoryManager
{
    public class ItemGatherSystem : MonoBehaviour
    {
        [SerializeField] private LayerMask _itemPickupLayerMask;
        [SerializeField] private LayerMask _resourceLayerMask;
        [Range(1f, 15f)][SerializeField] private float _collectibleDistance = 3.45f;
        [SerializeField] private float _cacheRefreshInterval = 0.5f;

        private InputManager _inputManager;
        private Camera _mainCamera;

        private float _lastCacheRefreshTime = 0f;
        private bool _isCollectableInRange = false;

        private RaycastHit _cachedHit;
        private Vector2 _centerOfScreen;

        private void Awake()
        {
            _inputManager = new InputManager();
            InitializeInputAction();
        }

        private void OnEnable()
        {
            _inputManager.Enable();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }
        private void InitializeInputAction()
        {
            _inputManager.Player.Interact.started += Pickup;
            _inputManager.Player.Attack.started += Collect;
        }
        private void DeinitializeInputAction()
        {
            _inputManager.Player.Attack.started -= Collect;
        }
        private void Update()
        {
            CheckSourceInRange();
        }
        private void CheckSourceInRange()
        {
            if (Time.time - _lastCacheRefreshTime >= _cacheRefreshInterval)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out _cachedHit, _collectibleDistance, _resourceLayerMask))
                {
                    SCSource scSource = _cachedHit.collider.gameObject.GetComponent<Source>().GetSource;
                    if (scSource != null)
                    {
                        if (InventoryManager.Instance.GetInventoryStatus)
                        {
                            GUIManager.Instance.storageCountTextMeshGUI.SetText($"Inventory full!");
                            GUIManager.Instance.stroageSliderGUI.gameObject.SetActive(false);
                            _isCollectableInRange = false;
                        }
                        else
                        {
                            UpdateUI(scSource);
                            _isCollectableInRange = true;
                        }
                    }
                    else
                    {
                        _isCollectableInRange = false;
                    }
                }
                else
                {
                    _isCollectableInRange = false;
                }

                GUIManager.Instance.collectionGUI.SetActive(_isCollectableInRange);
                GUIManager.Instance.crosshairGUI.SetActive(!_isCollectableInRange);
            }
        }
        private void Pickup(InputAction.CallbackContext context)
        {
            if (GUIManager.Instance.InventoryIsOpen) { return; }

            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _collectibleDistance, _itemPickupLayerMask, QueryTriggerInteraction.Ignore))
            {
                Item itemComponent = hit.collider.GetComponent<Item>();
                DroppedItem droppedItem = hit.collider.GetComponent<DroppedItem>();

                if (itemComponent != null || droppedItem != null)
                {
                    int itemID = itemComponent == null ? droppedItem.GetDroppedItemID : itemComponent.GetItem.GetItemID;
                    int itemCount = itemComponent == null ? droppedItem.GetDroppedItemCount : 1;

                    ItemDataManager.Instance.GetItemByID(itemID, itemCount);

                  //  Destroy(hit.collider.gameObject);
                }
            }
        }

        private void Collect(InputAction.CallbackContext context)
        {
            if (GUIManager.Instance.InventoryIsOpen) { return; }

            if (!_isCollectableInRange)
                return;

            SCSource scSource = _cachedHit.collider.gameObject.GetComponent<Source>().GetSource;

            if (InventoryManager.Instance.GetInventoryStatus)
            {
                GUIManager.Instance.storageCountTextMeshGUI.SetText($"Inventory full!");
                GUIManager.Instance.stroageSliderGUI.gameObject.SetActive(false);
                return;
            }

            UpdateUI(scSource);

            scSource.TakeDamage(2);

            if (scSource.gathered)
                Destroy(_cachedHit.collider.gameObject);

            _cachedHit = new RaycastHit();
            _isCollectableInRange = false;
        }
        private void UpdateUI(SCSource sCSource)
        {
            int sourceStorageCount = sCSource.storageCount;
            int sourceStorageMaxCount = sCSource.storageMaxCount;
            GUIManager.Instance.stroageSliderGUI.maxValue = sourceStorageMaxCount;
            GUIManager.Instance.stroageSliderGUI.value = sourceStorageCount;
            GUIManager.Instance.storageCountTextMeshGUI.SetText($"{sourceStorageCount}/{sourceStorageMaxCount}");
        }
        private void OnDisable()
        {
            _inputManager.Disable();
            DeinitializeInputAction();
        }
    }
}
