using Course.Core;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Utility
{
    public class JoystickInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField]
        private VirtualJoyStick joyStick = null;
        private IInputResolver _inputResolver;
        [SerializeField]
        private bool useJoyStick = true;

        private bool ShouldUseJoystick => useJoyStick && Application.isMobilePlatform;

        [Inject]
        private void Initialize(IInputResolver inputResolver)
        {
            _inputResolver = inputResolver;
            _inputResolver.SetJoystickEnabled(ShouldUseJoystick);
        }

        private void OnEnable()
        {
            if(joyStick == null || _inputResolver == null)
                return;
            joyStick.OnValueChanged += OnJoystickMoved;
        }
        
        private void OnDisable()
        {
            if(joyStick == null || _inputResolver == null)
                return;
            joyStick.OnValueChanged -= OnJoystickMoved;
            _inputResolver.ClearMove(InputSource.VirtualJoystick);
        }
        
        private void OnJoystickMoved(Vector2 dir)
        {
            _inputResolver?.SetMove(InputSource.VirtualJoystick, dir);
        }
        public void AwakeInitialize()
        {
            joyStick?.gameObject.SetActive(ShouldUseJoystick);
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