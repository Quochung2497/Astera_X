using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Course.ScriptableObject;
using Course.ScriptableObject.Item;
using Course.Services.SaveService;
using Course.Utility;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Services.Shop
{
    public class ShopManager : MonoBehaviour, IShopService, ISavable
    {
        [Header("Shop Configuration")]
        [SerializeField]
        private List<ShopEntry> shopEntries;

        [Inject] 
        private IPurse _purse;
        private IPaymentService _paymentService;

        private IShop _shop;

        private readonly List<ShipPartSO> _purchasedParts = new List<ShipPartSO>();
        public IEnumerable<ShipPartSO> PurchasedParts => _purchasedParts;
        public event Action<ShipPartSO> OnPartPurchased
        {
            add    => _shop.OnPartPurchased += value;
            remove => _shop.OnPartPurchased -= value;
        }
        
        public event Action OnShopUpdate;

        public event Action OnShopLoaded;

        public IEnumerable<Item> AllItems => shopEntries.Select(e => e.item);

        public IEnumerable<ShopEntry> Entries => shopEntries;

        public int GetAvailableStock(Item item)
            => _shop.GetAvailableStock(item);
        
        public bool IsSoldOut(Item item)
            => _shop.IsSoldOut(item);

        public bool CanBuy(Item item, int quantity = 1)
            => _shop.CanBuy(item, quantity);

        public bool Buy(Item item, int quantity = 1)
        {
            var success = _shop.Buy(item, quantity);
            if (success)
            {
                if (item is PremiumItem prem)
                    _purchasedParts.Add(prem.PartToUnlock);
                OnShopUpdate?.Invoke();
            }
            return success;
        }

        object ISavable.GetDefault() => new ShopSaveData();

        async Task ISavable.Load(ISaveService svc)
        {
            // Load saved purchases
            var defaultData = (ShopSaveData)((ISavable)this).GetDefault();
            var data = await svc.LoadData("ShopPurchases", defaultData, this);
            foreach (var rec in data.purchases)
            {
                _shop.ApplyPurchaseRecord(rec.itemID, rec.quantity);
                var entryItem = shopEntries
                    .First(e => e.item.ID == rec.itemID)
                    .item;
                if (entryItem is PremiumItem pi)
                    _purchasedParts.Add(pi.PartToUnlock);
            }
            OnShopLoaded?.Invoke();
        }

        async Task ISavable.Save(ISaveService svc)
        {
            // Build save records from current shop state
            var records = new List<PurchaseRecord>();
            foreach (var entry in shopEntries)
            {
                var boughtQty = _shop.GetPurchasedQuantity(entry.item.ID);
                if (boughtQty > 0)
                    records.Add(new PurchaseRecord { itemID = entry.item.ID, quantity = boughtQty });
            }

            var saveData = new ShopSaveData { purchases = records };
            await svc.SaveData("ShopPurchases", saveData, this);
        }

        // TODO: Temporary solution to initialize the payment service.
        private void Awake()
        {
            _paymentService = new PaymentServices();
            _shop = new Shop(
                shopEntries, 
                _purse, 
                _paymentService, 
                initiallyPurchasedIDs: Array.Empty<string>() 
            );
        }
    }
    [Serializable]
    public struct ShopEntry
    {
        public Item item;
        public bool isStockable;
        public int  quantity;
    }
    
    [Serializable]
    public class ShopSaveData
    {
        /// <summary>
        /// Records each purchased item along with the quantity bought.
        /// </summary>
        public List<PurchaseRecord> purchases = new List<PurchaseRecord>();
    }

    /// <summary>
    /// A simple struct to persist how many of each item have been bought.
    /// </summary>
    [Serializable]
    public struct PurchaseRecord
    {
        public string itemID;
        public int    quantity;
    }
}
