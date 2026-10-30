using System.Collections.Generic;
using Course.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility.DependencyInjection;

namespace Course.Installation
{
    [DefaultExecutionOrder(-900)]
    public class InputInstaller : MonoBehaviour
    {
        [Inject] private IInputResolver _inputResolver;
        private PlayerInputAdapter _directInput;
        private readonly List<RaycastResult> _uiRaycastResults = new();
        private EventSystem _eventSystem;
        private PointerEventData _pointerEventData;

        private void Awake()
        {
            _directInput = new PlayerInputAdapter(_inputResolver, IsPointerOverUI);
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

        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null) return false;

            if (_eventSystem != eventSystem || _pointerEventData == null)
            {
                _eventSystem = eventSystem;
                _pointerEventData = new PointerEventData(eventSystem);
            }

            _pointerEventData.Reset();
            _pointerEventData.position = screenPosition;
            _uiRaycastResults.Clear();
            eventSystem.RaycastAll(_pointerEventData, _uiRaycastResults);
            foreach (var result in _uiRaycastResults)
            {
                if (result.module is GraphicRaycaster) return true;
            }

            return false;
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            _inputResolver.SetEnabled(hasFocus);
            if (hasFocus) _directInput.Enable();
            else _directInput.Disable();
        }
    }
}
