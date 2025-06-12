using System;
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

        [Inject] 
        private IAsteraX _asteraX;
        
        public void AwakeInitialize()
        {

        }

        public void StartInitialize()
        {
            preLevelUI.Initialize(_level,_asteraX);
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
