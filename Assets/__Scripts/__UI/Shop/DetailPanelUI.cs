using Course.ScriptableObject.Item;
using Course.Services.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Course.UI.Shop
{
    public class DetailPanelUI : MonoBehaviour, IDetailPanel
    {
        [Header("Common UI Elements")]
        [SerializeField] protected Image            itemIcon;
        [SerializeField] protected TextMeshProUGUI itemTitleText;
        [SerializeField] protected TextMeshProUGUI itemDescriptionText;
        [SerializeField] private   Button           closeButton;

        protected virtual void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(HideDetail);
        }

        /// <summary>
        /// Show icon, title, and description for a generic item.
        /// </summary>
        public virtual void ShowDetail(Item item)
        {
            if (item == null)
            {
                HideDetail();
                return;
            }

            // Icon
            if (itemIcon != null)
            {
                itemIcon.sprite  = item.Icon;
                itemIcon.enabled = true;
            }

            // Title
            if (itemTitleText != null)
            {
                itemTitleText.text    = item.ItemName;
                itemTitleText.enabled = true;
            }

            // Description (assuming your Item has a Description field)
            if (itemDescriptionText != null)
            {
                itemDescriptionText.text    = item.Description;
                itemDescriptionText.enabled = true;
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Shop overload; default just shows basic info.
        /// </summary>
        public virtual void ShowDetail(Item item, IShopService shop)
        {
            ShowDetail(item);
        }

        /// <summary>
        /// Hide the detail panel.
        /// </summary>
        public virtual void HideDetail()
        {
            gameObject.SetActive(false);
        }
    }
}