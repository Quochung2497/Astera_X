using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    async void Awake()
    {
        // Initialize Unity Gaming Services
        await UnityServices.InitializeAsync();
        Debug.Log($"Analytics ready: {AnalyticsService.Instance}");
    }
}
