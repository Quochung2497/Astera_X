using System;
using Course.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Course.Utility
{
    public sealed class PlayerInputAdapter : System.IDisposable
    {
        private readonly IInputResolver _inputResolver;
        private readonly Func<Vector2, bool> _isPointerOverUI;
        private PlayerControls _controls;
        private bool _isEnabled;

        public PlayerInputAdapter(IInputResolver inputResolver, Func<Vector2, bool> isPointerOverUI)
        {
            _inputResolver = inputResolver ?? throw new ArgumentNullException(nameof(inputResolver));
            _isPointerOverUI = isPointerOverUI ?? throw new ArgumentNullException(nameof(isPointerOverUI));
            _controls = new PlayerControls();
        }

        public void Enable()
        {
            if (_controls == null || _isEnabled) return;

            _isEnabled = true;
            RegisterCallbacks();
            _controls.Player.Enable();
            _controls.Turret.Enable();
        }

        public void Disable()
        {
            if (!_isEnabled) return;

            _controls.Player.Disable();
            _controls.Turret.Disable();
            UnregisterCallbacks();
            _isEnabled = false;
            _inputResolver.ClearMove(InputSource.Direct);
        }

        public void Dispose()
        {
            Disable();
            _controls?.Dispose();
            _controls = null;
        }

        private void RegisterCallbacks()
        {
            _controls.Player.Move.performed += OnMove;
            _controls.Player.Move.canceled += OnMove;
            _controls.Turret.Aim.performed += OnAim;
            _controls.Turret.Aim.canceled += OnAim;
            _controls.Player.Shoot.performed += OnFire;
        }

        private void UnregisterCallbacks()
        {
            _controls.Player.Move.performed -= OnMove;
            _controls.Player.Move.canceled -= OnMove;
            _controls.Turret.Aim.performed -= OnAim;
            _controls.Turret.Aim.canceled -= OnAim;
            _controls.Player.Shoot.performed -= OnFire;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _inputResolver.SetMove(InputSource.Direct, context.ReadValue<Vector2>());
        }

        private void OnAim(InputAction.CallbackContext context)
        {
            _inputResolver.SetAim(context.ReadValue<Vector2>());
        }

        private void OnFire(InputAction.CallbackContext context)
        {
            if (!context.ReadValueAsButton()) return;

            Vector2 screenPosition;
            if (context.control.parent is TouchControl touch)
            {
                if (touch.phase.ReadValue() != UnityEngine.InputSystem.TouchPhase.Began) return;
                screenPosition = touch.position.ReadValue();
            }
            else if (context.control.device is Mouse mouse)
                screenPosition = mouse.position.ReadValue();
            else
                return;

            if (_isPointerOverUI(screenPosition)) return;
            _inputResolver.Fire(screenPosition);
        }
    }
}
