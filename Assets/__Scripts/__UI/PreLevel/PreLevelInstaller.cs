using Course.Core;
using Course.Level;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.UI
{
    public class PreLevelInstaller : MonoBehaviour , IInstaller
    {
        [SerializeField] 
        private PreLevelUI preLevelUI;

        [Inject] 
        private ILevel _level;

        public void AwakeInitialize()
        {

        }

        public void StartInitialize()
        {
            preLevelUI.Initialize(_level, _level.StartNextLevel);
        }

        private void Awake()
        {
            AwakeInitialize();
        }

        private void Start()
        {
            StartInitialize();
        }
    }
}
