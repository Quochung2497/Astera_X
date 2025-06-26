using Course.ScriptableObject.Item;
using Course.Services.Shop;

namespace Course.UI.Shop
{
    public interface IDetailPanel
    {
        /// <summary>
        /// Show basic item info (e.g. in inventory or elsewhere).
        /// </summary>
        void ShowDetail(Item item);
        /// <summary>
        /// Show the detail view for the given shop item.
        /// </summary>
        void ShowDetail(Item item, IShopService shop);

        /// <summary>
        /// Hide the detail view.
        /// </summary>
        void HideDetail();
    }
}