using System.Collections.Generic;
using System.Linq;
using Course.Core;
using Course.ObjectPool;
using Course.Services.Shop;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Utility.DependencyInjection;


namespace Course.UI.Shop
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private int rowSize = 4;
        [SerializeField] private ShopDetailUI detailPanel;

        private IShopService _shopService;


        private IFactory<ShopSlotUI> _slotPool;
        private readonly List<ShopSlotUI> _activeSlots = new();
        private bool _isInitialized = false;

        private void OnEnable()
        {
            if(!_isInitialized || _shopService == null || detailPanel == null)
                return;
            _shopService.OnShopUpdate += RebuildGrid;
        }

        private void OnDisable()
        {
            if(!_isInitialized || _shopService == null || detailPanel == null)
                return;
            _shopService.OnShopUpdate -= RebuildGrid;
        }

        /// <summary>Call once to show the shop</summary>
        public void Initialize(IShopService shop,IFactory<ShopSlotUI> uiFactory)
        {
            _shopService = shop;
            _slotPool = uiFactory;
            _shopService.OnShopUpdate += RebuildGrid;
            _isInitialized = true;
            RebuildGrid();
        }

        private void RebuildGrid()
        {
            if(!_isInitialized || _shopService == null || detailPanel == null)
                return;
            Debug.Log($"RebuildGrid called; initialized={_isInitialized}, shop={_shopService}, factory={_slotPool}");
            // return existing slots
            Debug.Log($"  Returning {_activeSlots.Count} active slots");
            foreach (var slot in _activeSlots)
                _slotPool.Release(slot);
            _activeSlots.Clear();

            // grab items
            var items = _shopService.AllItems.ToList();
            int count = items.Count;
            Debug.Log($"  Found {count} items in shop");
            int totalSlots = GetNextMultiple(count, rowSize);
            Debug.Log($"  Filling {totalSlots} slots (to make rows of {rowSize})");

            // real items
            for (int i = 0; i < count; i++)
            {
                var slot = _slotPool.Get();
                Debug.Log($"    Spawned item slot #{i}: {items[i].name} as {slot.name}");
                slot.Initialize(items[i], _shopService, detailPanel);
                _activeSlots.Add(slot);
            }

            int blankCount = totalSlots - count;
            // blank padders
            for (int i = 0; i < blankCount; i++)
            {
                var slot = _slotPool.Get();
                Debug.Log($"    Spawned blank slot #{i}");
                slot.Initialize(null, _shopService, detailPanel);
                _activeSlots.Add(slot);
            }
        }
        
        private int GetNextMultiple(int count, int rowSize)
        {
            if (rowSize <= 0) return count;
            if (count % rowSize == 0)
                return count;
            return ((count / rowSize) + 1) * rowSize;
        }
    }
}

