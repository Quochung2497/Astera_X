using System;
using System.Collections.Generic;
using System.Linq;
using Course.ScriptableObject;
using Course.ScriptableObject.Item;

namespace Course.Services.Shop
{
    public class Shop : IShop
    { 
        private readonly List<ShopEntry> _entries;
        private readonly IPurse          _purse;
        private readonly IPaymentService _paymentService;
        
        private readonly HashSet<string> _purchasedIDs = new();
        private Dictionary<string,int> _purchasedQuantities = new();
        
        public Shop(
            List<ShopEntry> entries,
            IPurse purse,
            IPaymentService paymentService,
            IEnumerable<string> initiallyPurchasedIDs = null
        )
        {
            _entries        = entries;
            _purse          = purse;
            _paymentService = paymentService;
        
            if (initiallyPurchasedIDs != null)
                foreach (var id in initiallyPurchasedIDs)
                    _purchasedIDs.Add(id);
        }
        
        public event Action<ShipPartSO> OnPartPurchased;
        public int CalculateTotalCost(Item item, int qty = 1)
            => item.Price * qty;
        
        public int GetAvailableStock(Item item)
        {
            var entry = _entries.FirstOrDefault(e => e.item == item);
            return entry.isStockable ? entry.quantity : int.MaxValue;
        }
        
        public bool IsSoldOut(Item item)
        {
            if (item.IsOneTimePurchaseOnly && _purchasedIDs.Contains(item.ID))
                return true;
        
            var entry = _entries.FirstOrDefault(e => e.item == item);
            if (entry.isStockable && entry.quantity <= 0)
                return true;
        
            return false;
        }
        
        public bool CanBuy(Item item, int qty = 1)
        {
            if (IsSoldOut(item)) return false;
            if (qty > GetAvailableStock(item)) return false;
            return _purse.CanAfford(CalculateTotalCost(item, qty));
        }
        
        /// <summary>
        /// Performs the purchase immediately. Returns true if successful.
        /// Persistence of the new purchase state is left up to the caller.
        /// </summary>
        public bool Buy(Item item, int qty = 1)
        {
            if (!CanBuy(item, qty) || item == null)
                return false;
        
            // Premium IAP path
            if (item is PremiumItem premium)
            {
                _paymentService.BuyItems(premium, qty);
                _purchasedIDs.Add(item.ID);
                _purchasedQuantities[item.ID] = qty;
                DecrementStock(item, qty);
                OnPartPurchased?.Invoke(premium.PartToUnlock);
                return true;
            }
        
            // In-game currency path
            _purse.Spend(CalculateTotalCost(item, qty));
            DecrementStock(item, qty);
            _purchasedQuantities[item.ID] = 
                    _purchasedQuantities.GetValueOrDefault(item.ID, 0) + qty;
            return true;
        }
        
        private void DecrementStock(Item item, int qty)
        {
            int idx = _entries.FindIndex(e => e.item == item);
            if (idx < 0) return;

            var entry = _entries[idx];
            if (entry.isStockable)
            {
                entry.quantity = Math.Max(0, entry.quantity - qty);
                _entries[idx]  = entry;
            }
        }
        
        public IEnumerable<string> GetPurchasedIDs() => _purchasedIDs;
        
        public void ApplyPurchaseRecord(string itemID, int qty)
        {
            // Record the one-time purchase flag
            _purchasedIDs.Add(itemID);

            // Record how many were bought for stackables
            _purchasedQuantities[itemID] = qty;

            // Decrement stock if applicable
            var idx = _entries.FindIndex(e => e.item.ID == itemID);
            if (idx >= 0 && _entries[idx].isStockable)
            {
                var entry = _entries[idx];
                entry.quantity = Math.Max(0, entry.quantity - qty);
                _entries[idx]  = entry;
            }
        }
        
        public int GetPurchasedQuantity(string itemID) => _purchasedQuantities.TryGetValue(itemID, out var q) ? q : 0;

    }
}