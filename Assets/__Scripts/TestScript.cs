using System;
using Course.Core;
using Course.ScriptableObject;
using Course.ScriptableObject.Item;
using Course.Services.Shop;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using Utility.DependencyInjection;


public class TestScript : MonoBehaviour
{

    [SerializeField]
    private Item itemToTest;
    [SerializeField]
    private Item PremiumItemToTest;
    [Inject]
    private IShopService _shop;
#if UNITY_EDITOR
    private void Update()
    {
        
        if(GameManager.TryGetInstance().currentStates != GameState.level) return;
        if(Input.GetKeyDown(KeyCode.Space))
            UnityServiceBootstrap.TryGetInstance().RunAds();
        if(Input.GetKeyDown(KeyCode.B))
            UnityServiceBootstrap.TryGetInstance().RunBanner();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            bool ok = _shop.Buy(itemToTest, 1);
            Debug.Log($"Tried buying Potion: {(ok? "SUCCESS" : "FAIL")} | Remaining stock: {_shop.GetAvailableStock(itemToTest)}");
        }
        
        // Press 2 to buy the Turbo unlock (IAP, one-time)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bool ok = _shop.Buy(PremiumItemToTest, 1);
            Debug.Log($"Tried buying Turbo Unlock: {(ok? "SUCCESS" : "FAIL")}");
        }
    }
#endif
}
