using Course.Utility;
using UnityEngine;

namespace Course.UI.Shop
{
    public class ShopSlotUIPool : Pool<ShopSlotUI>
    {
        private readonly Transform _parent;

        /// <summary>
        /// Constructs a new pool.
        /// </summary>
        /// <param name="prefab">The ShopSlotUI prefab to clone.</param>
        /// <param name="parent">The RectTransform under which to parent pooled slots.</param>
        /// <param name="maxSize">Maximum number of objects this pool will hold.</param>
        /// <param name="defaultCapacity">Initial capacity of the pool.</param>
        public ShopSlotUIPool(
            ShopSlotUI prefab,
            Transform parent,
            int maxSize = 16,
            int defaultCapacity = 16
        ) : base(prefab, true, maxSize, defaultCapacity)
        {
            _parent = parent;
        }

        /// <summary>
        /// Creates a fresh ShopSlotUI by instantiating the prefab.
        /// </summary>
        protected override ShopSlotUI OnCreate()
        {
            var slotUI = GameObject.Instantiate(Prefab, _parent, false);
            slotUI.SetPool(PoolInstance);
            return slotUI;
        }
    }
}