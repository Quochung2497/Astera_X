using System;
using Course.Attribute;
using Course.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Course.UI
{
    public class CanvasInstaller : MonoBehaviour, IInstaller
    {
        [Header("Dependencies Injection")] 
        [SerializeField]
        private ScoreBehaviour score;
        [FormerlySerializedAs("jump")] [SerializeField]
        private HealthBehaviour health;
        
        [Header("GUI Components")]
        [SerializeField]
        private AttributeUI scoreUI;
        [SerializeField]
        private AttributeUI jumpUI;
        [SerializeField]
        private GameOverUI gameOverUI;
        
        public void AwakeInitialize() { }

        public void StartInitialize()
        {
            scoreUI.Initialize(score);
            jumpUI.Initialize(health);
            gameOverUI.Initialize(score, 0);
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
