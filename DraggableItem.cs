using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventoryManager
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Rect _tooltipRectPos;
        private Transform _parentAfterDrag;
        private InventorySlot _previousSlot;
        private int _previousItemCount;
        private Image _image;
        public float width;
        public float height;
        internal Transform GetParentAfterDrag { get => _parentAfterDrag; set => _parentAfterDrag = value; }
        internal InventorySlot GetPreviousSlot { get => _previousSlot; set => _previousSlot = value; }
        internal int GetPreviousItemCount { get => _previousItemCount; }
        private void Start()
        {
            _image = GetComponent<Image>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            _parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            _image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;

            _previousSlot = _parentAfterDrag.GetComponent<InventorySlot>();
            _previousItemCount = _previousSlot.GetCountOfItemInSlot;
            _previousItemCount = _previousSlot.GetCountOfItemInSlot;

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(_parentAfterDrag);
            _image.raycastTarget = true;

            InventorySlot newSlot = _parentAfterDrag.GetComponent<InventorySlot>();

            if (newSlot != _previousSlot)
            {
                if (_previousSlot.transform.childCount == 0)
                {
                    _previousSlot.GetCountOfItemInSlot = 0;
                    newSlot.GetCountOfItemInSlot += _previousItemCount;
                }
            }
        }
    }

}
