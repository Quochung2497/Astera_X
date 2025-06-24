using System;
using System.Collections.Generic;
using Course.ScriptableObject;
using Course.ScriptableObject.Item;

namespace Course.Services.Shop
{
    public interface IShopService
    {
        /// <summary>
        /// All items available for purchase in this shop.
        /// </summary>
        IEnumerable<Item> AllItems { get; }

        /// <summary>
        /// Retrieves the stock entries for items in this shop.
        /// </summary>
        IEnumerable<ShopEntry> Entries { get; }

        /// <summary>
        /// Returns how many of the given item are currently in stock (int.MaxValue if unlimited).
        /// </summary>
        int GetAvailableStock(Item item);

        /// <summary>
        /// Returns whether the player can buy the specified quantity of the item.
        /// </summary>
        bool CanBuy(Item item, int quantity = 1);

        /// <summary>
        /// Attempts to purchase the specified quantity of the item. Returns true on success.
        /// </summary>
        bool Buy(Item item, int quantity = 1);

        /// <summary>
        /// Fires when a premium purchase unlocks a ShipPartSO.
        /// </summary>
        event Action<ShipPartSO> OnPartPurchased;
        
        event Action OnShopUpdate;
        
        /// <summary> All parts the user has bought so far. </summary>
        IEnumerable<ShipPartSO> PurchasedParts { get; }
    }
}