using System;
using UnityEngine;

namespace Course.Utility
{
    public enum InputSource
    {
        Direct,
        VirtualJoystick,
        Tilt
    }

    public sealed class InputResolver : IInputResolver, IInputReader
    {
        private Vector2 _directMove;
        private Vector2 _joystickMove;
        private Vector2 _tiltMove;

        public Vector2 MoveDirection { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsJoystickEnabled { get; private set; }

        public event Action<Vector2> OnMoveEvent;
        public event Action<Vector2> OnAimEvent;
        public event Action<Vector2> OnFireEvent;

        public void SetMove(InputSource source, Vector2 direction)
        {
            if (!IsEnabled) return;

            switch (source)
            {
                case InputSource.Direct:
                    _directMove = direction;
                    break;
                case InputSource.VirtualJoystick:
                    if (!IsJoystickEnabled) return;
                    _joystickMove = direction;
                    break;
                case InputSource.Tilt:
                    if (IsJoystickEnabled) return;
                    _tiltMove = direction;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }

            ResolveMove();
        }

        public void ClearMove(InputSource source)
        {
            SetMove(source, Vector2.zero);
        }

        public void SetAim(Vector2 screenPosition)
        {
            if (IsEnabled) OnAimEvent?.Invoke(screenPosition);
        }

        public void Fire(Vector2 screenPosition)
        {
            if (IsEnabled) OnFireEvent?.Invoke(screenPosition);
        }

        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;
            if (enabled) return;

            _directMove = _joystickMove = _tiltMove = Vector2.zero;
            ResolveMove();
        }

        public void SetJoystickEnabled(bool enabled)
        {
            if (IsJoystickEnabled == enabled) return;

            IsJoystickEnabled = enabled;
            _joystickMove = _tiltMove = Vector2.zero;
            ResolveMove();
        }

        private void ResolveMove()
        {
            var direction = _directMove;
            if (direction == Vector2.zero)
                direction = IsJoystickEnabled ? _joystickMove : _tiltMove;

            if (direction == MoveDirection) return;

            MoveDirection = direction;
            OnMoveEvent?.Invoke(direction);
        }
    }
}
