using System;
using System.Collections.Generic;
using Course.ScriptableObject;
using Course.ScriptableObject.Item;

namespace Course.Services.Shop
{
    public interface IShop
    {
        event Action<ShipPartSO> OnPartPurchased;
        int CalculateTotalCost(Item item, int qty);
        int GetAvailableStock(Item item);
        bool CanBuy(Item item, int qty = 1);
        bool IsSoldOut(Item item);
        bool Buy(Item item, int qty = 1);
        IEnumerable<string> GetPurchasedIDs();
        void ApplyPurchaseRecord(string itemID, int qty);
        int GetPurchasedQuantity(string itemID);
    }
}