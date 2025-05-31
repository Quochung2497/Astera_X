using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(fileName = "PlayerShipConfig", menuName = "Scriptable Objects/PlayerShipConfig")]
    public class PlayerShipConfig : UnityEngine.ScriptableObject
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 10f;
        [field: SerializeField] public float JumpCount { get; private set; } = 3f;
        [field: SerializeField] public float Score { get; private set; } = 5f;
        [field: SerializeField] public float MaxTiltAngle { get; private set; } = 30f;
        [field: SerializeField] public float TiltSpeed { get; private set; } = 5f;
    }
}
