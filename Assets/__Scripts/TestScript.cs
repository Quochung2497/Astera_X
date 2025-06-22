using Course.Core;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Update()
    {
        if(GameManager.TryGetInstance().currentStates != GameState.level) return;
        if(Input.GetKeyDown(KeyCode.Space))
            UnityServiceBootstrap.TryGetInstance().RunAds();
        if(Input.GetKeyDown(KeyCode.B))
            UnityServiceBootstrap.TryGetInstance().RunBanner();
    }
}
