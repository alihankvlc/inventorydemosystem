using SurvivalProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryManager
{
    [System.Serializable]
    public class SCSource : IDamageable
    {
        public SCItem giveItem;
        public int storageCount;
        public int storageMaxCount;
        public int damageAccumulator;
        public bool gathered;
        public void TakeDamage(int amount)
        {

            if (storageCount >= 1)
            {
                if (amount < storageCount)
                {
                    storageCount -= amount;
                    damageAccumulator += amount;
                    if (damageAccumulator >= 7)
                    {
                        ItemDataManager.Instance.GetItemByID(2, 1);
                        damageAccumulator = 0;
                    }
                }
                else
                {
                    ItemDataManager.Instance.GetItemByID(2, 5);
                    gathered = true;
                }
            }
        }
    }

}
