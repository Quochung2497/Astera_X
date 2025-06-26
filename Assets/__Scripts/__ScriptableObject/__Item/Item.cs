using System;
using UnityEngine;

namespace Course.ScriptableObject.Item
{
    public abstract class Item : UnityEngine.ScriptableObject
    {
        [Tooltip("Auto-generated UUID for save/load; clear to regenerate")]
        [SerializeField] 
        private string itemID = Guid.NewGuid().ToString();

        [Tooltip("Icon for this item, shown in the shop and inventory")]
        [Header("Visuals")]
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [Tooltip("Name of the item, shown in the shop and inventory")]
        [field: SerializeField] public String ItemName { get; private set; }
        
        [Tooltip("Description of the item, shown in the shop and inventory")]
        [field: SerializeField] public string Description { get; private set; }
        
        
        [Header("Pricing")]
        [Tooltip("Price in your chosen currency")]
        [field: SerializeField] 
        public int Price { get; private set; }

        [Header("Purchase Rules")]
        [Tooltip("True if this item can only be bought once")]
        [field: SerializeField] 
        public bool IsOneTimePurchaseOnly { get; private set; }

        [Tooltip("True if this item can be bought multiple times (stackable)")]
        [field: SerializeField]
        public bool IsStackable { get; private set; }

        /// <summary>
        /// Unique identifier used for saving and lookup.
        /// </summary>
        public string ID => itemID;
    }
}