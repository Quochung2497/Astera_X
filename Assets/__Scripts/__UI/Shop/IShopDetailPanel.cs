namespace Course.UI.Shop
{
    /// <summary>
    /// Additional shop‐only actions (quantity, buy/cancel).
    /// </summary>
    public interface IShopDetailPanel : IDetailPanel
    {
        /// <summary>
        /// Called when the “+” button is clicked.
        /// </summary>
        void OnPlusClicked();

        /// <summary>
        /// Called when the “–” button is clicked.
        /// </summary>
        void OnMinusClicked();

        /// <summary>
        /// Called when the “Buy” button is clicked.
        /// </summary>
        void OnBuyClicked();

        /// <summary>
        /// Called when the “Cancel” button is clicked.
        /// </summary>
        void OnCancelClicked();
    }
}