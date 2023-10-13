using SurvivalProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventoryManager
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private SCItem _item;
        internal SCItem GetItem { get => _item; set => _item = value; }
    }
}
