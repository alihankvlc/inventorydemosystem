using InventoryManager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace InventoryManager
{
    public class DroppedItem : MonoBehaviour
    {
        public int GetDroppedItemID { get; internal set; }
        [field: SerializeField]
        public int GetDroppedItemCount { get; internal set; }

        private const float DESTROY_TIME = 60f;
        private void Start()
        {
            StartCoroutine(ItemLostCoroutine());
        }
        private IEnumerator ItemLostCoroutine()
        {
            while (true)
            {
                WaitForSeconds destoryTime = new WaitForSeconds(DESTROY_TIME);

                yield return destoryTime;

                Destroy(this.gameObject);
                break;
            }
        }
    }
}


