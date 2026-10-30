using Course.Utility;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Installation
{
    [DefaultExecutionOrder(-900)]
    public class InputInstaller : MonoBehaviour
    {
        [Inject] private IInputResolver _inputResolver;
        private PlayerInputAdapter _directInput;

        private void Awake()
        {
            _directInput = new PlayerInputAdapter(_inputResolver);
        }

        private void OnEnable()
        {
            Application.focusChanged += OnApplicationFocusChanged;
            OnApplicationFocusChanged(Application.isFocused);
        }

        private void OnDisable()
        {
            Application.focusChanged -= OnApplicationFocusChanged;
            _inputResolver?.SetEnabled(false);
            _directInput?.Disable();
        }

        private void OnDestroy()
        {
            _directInput?.Dispose();
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            _inputResolver.SetEnabled(hasFocus);
            if (hasFocus) _directInput.Enable();
            else _directInput.Disable();
        }
    }
}
