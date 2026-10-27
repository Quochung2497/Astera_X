using Course.Core;
using Course.UI;
using UnityEngine;

namespace Course.Installation
{
    public class PausePanelInstaller : MonoBehaviour , IInstaller
    {
        [SerializeField]
        private ActionButton pauseButton;

        private GameManager _gameManager;
        
        public void AwakeInitialize()
        { }

        public void StartInitialize()
        {
            _gameManager = GameManager.TryGetInstance();
            if (_gameManager == null) return;
            pauseButton?.Initialize(_gameManager.TogglePause);
        }

        private void Start()
        {
            StartInitialize();
        }
    }
}

