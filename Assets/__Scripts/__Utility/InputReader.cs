using System;
using Course.Input;
using UnityEngine;

namespace Course.Utility
{
    /// <summary>
    /// A ScriptableObject that serves as an input reader for player controls.
    /// Handles movement, aiming, and firing events, and provides initialization and shutdown functionality.
    /// </summary>
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
    public class InputReader : UnityEngine.ScriptableObject, IInputReader
    {
        /// <summary>
        /// Cached instance of the player controls.
        /// </summary>
        private PlayerControls _controls;

        #region IIputReaderAPI

        /// <summary>
        /// Event triggered when the player moves.
        /// Provides a Vector2 representing the movement direction.
        /// </summary>
        public event Action<Vector2> OnMoveEvent;

        /// <summary>
        /// Event triggered when the player aims.
        /// Provides a Vector2 representing the aiming direction.
        /// </summary>
        public event Action<Vector2> OnAimEvent;

        /// <summary>
        /// Event triggered when the player fires.
        /// </summary>
        public event Action OnFireEvent;

        /// <summary>
        /// Initializes the input reader and sets up event bindings for player controls.
        /// </summary>
        public void Initialize()
        {
            if (_controls != null) return;  // already initialized

            _controls = new PlayerControls();

            // MOVE (WASD / stick / touch-drag / tilt)
            _controls.Player.Move.performed += ctx =>
                OnMoveEvent?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Player.Move.canceled += ctx =>
                OnMoveEvent?.Invoke(Vector2.zero);

            // AIM (mouse / touch-pos / right stick)
            _controls.Turret.Aim.performed += ctx =>
                OnAimEvent?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Turret.Aim.canceled += ctx =>
                OnAimEvent?.Invoke(Vector2.zero);

            // FIRE (click / button / tap)
            _controls.Player.Shoot.performed += _ =>
                OnFireEvent?.Invoke();

            // Enable the control maps
            _controls.Player.Enable();
            _controls.Turret.Enable();
        }

        /// <summary>
        /// Shuts down the input reader and disables player controls.
        /// Unsubscribes from all events and clears the cached controls.
        /// </summary>
        public void Shutdown()
        {
            if (_controls == null) return;

            _controls.Player.Disable();
            _controls.Turret.Disable();

            // Unsubscribe from events (optional but clean)
            _controls.Player.Move.performed -= ctx => OnMoveEvent?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Player.Move.canceled -= ctx => OnMoveEvent?.Invoke(Vector2.zero);
            _controls.Turret.Aim.performed -= ctx => OnAimEvent?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Turret.Aim.canceled -= ctx => OnAimEvent?.Invoke(Vector2.zero);
            _controls.Player.Shoot.performed -= _ => OnFireEvent?.Invoke();

            _controls = null;
        }
        #endregion
    }
}

