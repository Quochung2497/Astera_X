using Course.Core;
using Course.Services.SaveService;
using Course.Utility.Events;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.UI.SaveUI
{
    public class SavePanelInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField]
        private ActionButton deleteSaveButton;

        [Inject] 
        private ISaveManager _saveManager;

        public void AwakeInitialize()
        {
// #if !UNITY_EDITOR
//             deleteSaveButton.gameObject.SetActive(false);
// #endif
        }

        public void StartInitialize()
        {
            // guard in case injection failed
            
            if(_saveManager == null)
            {
                Debug.LogError("[SavePanelInstaller] SaveManager is not injected properly.");
                return;
            }
            if (deleteSaveButton == null || _saveManager == null) return;

            deleteSaveButton.Initialize(() =>
            {
                Debug.Log("[SavePanel] Deleting all save data…");
                _saveManager.DeleteAll();
            });
        }

        private void Start()
        {
            StartInitialize();
        }
    }
}