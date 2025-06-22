using System;
using Course.Core;
using UnityEngine;

namespace Course.Utility
{
    public class JoystickInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField]
        private VirtualJoyStick joyStick = null;
        [SerializeField]
        private InputReader inputReader = null;
        [SerializeField]
        private bool useJoyStick = true;

        private void OnEnable()
        {
            if(joyStick == null || inputReader == null)
                return;
            joyStick.OnValueChanged += OnJoystickMoved;
        }
        
        private void OnDisable()
        {
            if(joyStick == null || inputReader == null)
                return;
            joyStick.OnValueChanged -= OnJoystickMoved;
        }
        
        private void OnJoystickMoved(Vector2 dir)
        {
            inputReader?.FeedMove(dir);
        }
        public void AwakeInitialize()
        {
            joyStick?.gameObject.SetActive(useJoyStick);
        }
        
        public void StartInitialize()
        {
        }

        private void Awake()
        {
            AwakeInitialize();
        }
    }
}