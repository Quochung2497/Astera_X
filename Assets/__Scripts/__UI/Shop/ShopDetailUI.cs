using System.Collections;
using Course.ScriptableObject.Item;
using Course.Services.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Course.UI.Shop
{
    public class ShopDetailUI : DetailPanelUI, IShopDetailPanel
    {
        [Header("Shop Controls")]
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Button           plusButton;
        [SerializeField] private Button           minusButton;
        [SerializeField] private TextMeshProUGUI totalCostText;
        [SerializeField] private Image            currencyIconImage;
        [SerializeField] private Button           buyButton;
        [SerializeField] private TextMeshProUGUI buyButtonText;
        [SerializeField] private Button           cancelButton;
        [SerializeField] private GameObject spinner;
        [SerializeField] private float duration = 1f;

        [Header("Error Notification")]
        [SerializeField] private GameObject       failedPanel;
        [SerializeField] private TextMeshProUGUI  failedText;
        [SerializeField] private Button           closeErrorButton;

        private IShopService _shop;
        private Item         _item;
        private int          _quantity;

        private const string SOLD_OUT_MSG = "Item is sold out";
        private const string FAIL_MSG     = "Failed to purchase";

        protected override void Awake()
        {
            base.Awake();
            plusButton?.onClick.AddListener(OnPlusClicked);
            minusButton?.onClick.AddListener(OnMinusClicked);
            buyButton?.onClick.AddListener(OnBuyClicked);
            cancelButton?.onClick.AddListener(OnCancelClicked);
            closeErrorButton?.onClick.AddListener(() => SetError(null));
        }

        /// <summary>
        /// Show the panel for a shop purchase.
        /// </summary>
        public override void ShowDetail(Item item, IShopService shop)
        {
            _item     = item;
            _shop     = shop;
            _quantity = 1;

            base.ShowDetail(item, shop);
            SetError(null);
            RefreshAll();
        }

        /// <summary>
        /// Basic ShowDetail overload (non-shop).
        /// </summary>
        public override void ShowDetail(Item item)
        {
            base.ShowDetail(item);
            // no shop controls in this mode
        }

        private void RefreshAll()
        {
            RefreshQuantity();
            RefreshCost();
            RefreshButtons();
        }

        private void RefreshQuantity()
        {
            if (quantityText != null)
                quantityText.text = _quantity.ToString();
        }

        private void RefreshCost()
        {
            if (_item != null && totalCostText != null)
            {
                int unitPrice = _item.Price;
                totalCostText.text = (unitPrice * _quantity).ToString();
            }

            if (currencyIconImage != null)
                currencyIconImage.enabled = (_item != null);
        }

        private void RefreshButtons()
        {
            if (_item == null || _shop == null) return;

            int available     = _shop.GetAvailableStock(_item);
            bool isSoldOut    = _shop.IsSoldOut(_item);
            bool canMinus     = _quantity > 1;
            bool canPlus      = available == int.MaxValue || _quantity < available;

            minusButton.interactable = canMinus;
            plusButton.interactable  = canPlus;
            buyButton.interactable   = !isSoldOut;
            spinner.SetActive(false);
        }

        public void OnPlusClicked()
        {
            _quantity++;
            RefreshAll();
        }

        public void OnMinusClicked()
        {
            if (_quantity > 1)
                _quantity--;
            RefreshAll();
        }

        public void OnBuyClicked()
        {
            if (_item == null || _shop == null) return;

            bool success = _shop.Buy(_item, _quantity);
            if (success)
            {
                StartCoroutine( ShowSpinner());
            }
            else
            {
                SetError(_shop.IsSoldOut(_item) ? SOLD_OUT_MSG : FAIL_MSG);
            }
        }

        public void OnCancelClicked()
        {
            HideDetail();
        }

        /// <summary>
        /// Displays or clears the error message.
        /// </summary>
        private void SetError(string message)
        {
            if (failedPanel != null)
                failedPanel.SetActive(!string.IsNullOrEmpty(message));
            if (failedText != null)
                failedText.text = message ?? "";
        }
        
        private IEnumerator ShowSpinner()
        {
            if (spinner != null)
            {
                buyButtonText.gameObject.SetActive(false);
                spinner.SetActive(true);
                yield return new WaitForSecondsRealtime(duration); // Simulate loading time
                spinner.SetActive(false);
                buyButtonText.gameObject.SetActive(true);
                HideDetail();
            }
        }
    }
}