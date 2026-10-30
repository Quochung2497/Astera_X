using UnityEngine;

namespace Course.Utility
{
    public interface IInputResolver
    {
        bool IsEnabled { get; }
        bool IsJoystickEnabled { get; }
        void SetMove(InputSource source, Vector2 direction);
        void ClearMove(InputSource source);
        void SetAim(Vector2 screenPosition);
        void Fire(Vector2 screenPosition);
        void SetEnabled(bool enabled);
        void SetJoystickEnabled(bool enabled);
    }
}
