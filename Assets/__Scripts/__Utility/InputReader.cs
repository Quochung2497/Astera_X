using System;
using Course.Input;
using UnityEngine;

namespace Course.Utility
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
    public class InputReader : ScriptableObject, IInputReader
    {
        // Cache
        private PlayerControls _controls;

        #region IIputReaderAPI

        public event Action<Vector2> OnMoveEvent;
        public event Action<Vector2> OnAimEvent;
        public event Action         OnFireEvent;
        
        public void Initialize()
        {
            if (_controls != null) return;  // already inited

            _controls = new PlayerControls();

            // MOVE (WASD / stick / touch‐drag / tilt)
            _controls.Player.Move.performed += ctx =>
                OnMoveEvent?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Player.Move.canceled  += ctx =>
                OnMoveEvent?.Invoke(Vector2.zero);

            // AIM (mouse / touch‐pos / right stick)
            _controls.Turret.Aim.performed  += ctx =>
                OnAimEvent?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Turret.Aim.canceled   += ctx =>
                OnAimEvent?.Invoke(Vector2.zero);

            // FIRE (click / button / tap)
            _controls.Player.Shoot.performed += _ =>
                OnFireEvent?.Invoke();

            // turn the map on
            _controls.Player.Enable();
            _controls.Turret.Enable();
        }

        public void Shutdown()
        {
            if (_controls == null) return;

            _controls.Player.Disable();
            _controls.Turret.Disable();

            // unsubscribe (optional but clean)
            _controls.Player.Move.performed -= ctx => OnMoveEvent?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Player.Move.canceled  -= ctx => OnMoveEvent?.Invoke(Vector2.zero);
            _controls.Turret.Aim.performed  -= ctx => OnAimEvent?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Turret.Aim.canceled   -= ctx => OnAimEvent?.Invoke(Vector2.zero);
            _controls.Player.Shoot.performed-= _   => OnFireEvent?.Invoke();

            _controls = null;
        }
        #endregion
 
    }
}

