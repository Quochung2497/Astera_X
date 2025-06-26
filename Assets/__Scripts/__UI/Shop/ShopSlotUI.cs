using Course.ScriptableObject.Item;
using Course.Services.Shop;
using Course.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Course.UI.Shop
{
    [RequireComponent(typeof(Button))]
    public class ShopSlotUI : MonoBehaviour, IPoolObject<ShopSlotUI>
    {
        [Header("UI Elements")]
        [SerializeField] private Image      iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button     button;

        private IObjectPool<ShopSlotUI> _pool;
        private Item            _item;
        private IShopService    _shopService;
        private IDetailPanel    _detailPanel;

        /// <summary>
        /// Called by ShopUI when a slot is (re)used.
        /// </summary>
        public void Initialize(
            Item item,
            IShopService shopService,
            IDetailPanel detailPanel)
        {
            if (iconImage == null || nameText == null || priceText == null || button == null)
            {
                Debug.LogError($"[ShopSlotUI] missing inspector reference on {name}");
                return;
            }
            _item         = item;
            _shopService  = shopService;
            _detailPanel  = detailPanel;

            if (_item == null)
            {
                // Blank placeholder
                iconImage.enabled        = false;
                nameText.enabled         = false;
                priceText.enabled        = false;
                button.interactable      = false;
                return;
            }

            // 1) Icon & Name
            iconImage.enabled = true;
            iconImage.sprite  = _item.Icon;

            nameText.enabled  = true;
            nameText.text     = _item.ItemName; 

            // 2) Price
            priceText.enabled = true;
            priceText.text    = _item.Price.ToString();

            // 3) Availability
            bool soldOut = _shopService.IsSoldOut(_item);

            // 4) Button callback
            button.interactable = !soldOut;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                _detailPanel.ShowDetail(_item, _shopService);
            });
        }

        /// <summary>
        /// Store a reference to our pool so we can return ourselves.
        /// </summary>
        public void SetPool(IObjectPool<ShopSlotUI> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Reset this slot before returning it to the pool.
        /// </summary>
        public void Reset()
        {
            // Unsubscribe button
            button.onClick.RemoveAllListeners();

            // Clear references
            _item        = null;
            _shopService = null;
            _detailPanel = null;

        }

        /// <summary>
        /// Return this slot back to its pool.
        /// </summary>
        public void Release()
        {
            _pool?.Release(this);
        }
    }
}