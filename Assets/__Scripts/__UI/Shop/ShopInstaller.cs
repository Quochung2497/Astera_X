using System;
using Course.Core;
using Course.ObjectPool;
using Course.Services.Shop;
using UnityEngine;
using UnityEngine.Pool;
using Utility.DependencyInjection;

namespace Course.UI.Shop
{
    public class ShopInstaller : MonoBehaviour, IInstaller
    {
        [Header("References (assign in inspector)")]
        [SerializeField] private ShopUI         shopUI = null;
        [SerializeField] private ShopSlotUI     slotPrefab = null;
        [SerializeField] private RectTransform content = null;
        [SerializeField] private ActionButton shopToggleButton = null;
        [SerializeField] private int            poolMaxSize       = 32;
        [SerializeField] private int            poolInitialSize   = 16;

        [Inject]
        private IShopService _shopService;
        
        private IObjectPool<ShopSlotUI> _slotPool;
        private IFactory<ShopSlotUI> _slotFactory;
        private GameManager  _gameManager;
        
        public void AwakeInitialize()
        {
            if (_shopService == null) return;
            var concretePool = new ShopSlotUIPool(slotPrefab, content, poolMaxSize, poolInitialSize);
            Debug.Log($"Pool created – prefab={slotPrefab.name}, pool={concretePool.GetPool()}");
            _slotPool    = concretePool.GetPool();
            _slotFactory = new ShopSlotUIFactory(_slotPool);
            _shopService.OnShopLoaded += ShopInitialize;
            Debug.Log("ShopInstaller.AwakeInitialize");
        }

        public void StartInitialize()
        {
            _gameManager = GameManager.TryGetInstance();
            if (shopToggleButton == null || shopUI == null)
            {
                Debug.LogError("[ShopToggleInstaller] Missing references!");
                return;
            }
            if (_gameManager == null || _shopService == null)
            {
                Debug.LogError("[ShopToggleInstaller] Missing GameManager or ShopService!");
                return;
            }

            shopToggleButton.Initialize(_gameManager.ToggleShop);
        }

        private void Awake()
        {
            AwakeInitialize();
        }

        private void Start()
        {
            StartInitialize();
        }

        private void OnDestroy()
        {
            if (_shopService == null) return;
            _shopService.OnShopLoaded -= ShopInitialize;
        }

        private void ShopInitialize()
        {
            shopUI?.Initialize(_shopService, _slotFactory);
        }
    }
}