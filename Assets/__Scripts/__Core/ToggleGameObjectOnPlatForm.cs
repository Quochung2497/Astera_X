using System;
using UnityEngine;

namespace Course.Core
{
    public class ToggleGameObjectOnPlatForm : MonoBehaviour
    {
        [SerializeField] private GameObject[] prefabs;
        [SerializeField] private GameObject[] mobilePrefabs;
        #if UNITY_EDITOR
        [SerializeField] private bool isMobilePlatform = false; // for editor testing only
        #endif

        private void Awake()
        {
            bool onMobile = Application.isMobilePlatform
#if UNITY_EDITOR
                            || isMobilePlatform
#endif
                ;

            var activeSet   = onMobile ? mobilePrefabs : prefabs;
            var inactiveSet = onMobile ? prefabs       : mobilePrefabs;

            // Safety: if one set is null/empty, just leave everything else alone
            if (activeSet == null || activeSet.Length == 0) 
                return;

            // Enable the “active” prefabs
            foreach (var go in activeSet)
            {
                if (go != null) go.SetActive(true);
            }

            // Disable the opposite set (if it exists)
            if (inactiveSet != null && inactiveSet.Length > 0)
            {
                foreach (var go in inactiveSet)
                {
                    if (go != null) go.SetActive(false);
                }
            }
        }
    }
}
