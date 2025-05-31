using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(fileName = "BulletConfig", menuName = "Scriptable Objects/BulletConfig")]
    public class BulletConfig : UnityEngine.ScriptableObject
    {
        [field: SerializeField] public float speed { get; private set; } = 20f;
        [field: SerializeField] public float lifetime { get; private set; } = 2f;
        [field: SerializeField] public float damage { get; private set; } = 1f;
    }
}
