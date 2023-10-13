using InventoryManager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryManager
{
    public class Source : MonoBehaviour
    {
        [SerializeField] private SCSource _source;
        internal SCSource GetSource { get => _source; set => _source = value; }
    }
}
