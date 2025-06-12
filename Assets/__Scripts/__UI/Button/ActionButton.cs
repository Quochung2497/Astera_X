using System;
using UnityEngine;
using UnityEngine.UI;

namespace Course.UI
{
    [RequireComponent(typeof(Button))]
    public class ActionButton : MonoBehaviour
    {
        [Header("UI Components")]
        [Tooltip("Drag the Unity UI Button component here (or let RequireComponent auto-assign).")]
        [SerializeField] private Button button;

        private Image   _image; 
        private Action _onClickAction;
        private bool _initialized = false;

        private void OnEnable()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (button == null)
                Debug.LogError($"[ActionButton] No Button found on {gameObject.name}!");
            if(_initialized)
                button?.onClick.AddListener(() => _onClickAction?.Invoke());
        }

        /// <summary>
        /// Call this exactly once to set what should happen when the button is clicked.
        /// </summary>
        public void Initialize(Action onClick)
        {
            if (_initialized) return;
            _onClickAction = onClick;
            button?.onClick.AddListener(() => _onClickAction?.Invoke());
            _initialized = true;
        }

        private void OnDisable()
        {
            if (button != null && _initialized)
                button?.onClick.RemoveAllListeners();
        }
    }
}
