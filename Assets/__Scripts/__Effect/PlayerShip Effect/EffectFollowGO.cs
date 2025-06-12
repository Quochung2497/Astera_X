using UnityEngine;

namespace Course.Effect
{
    public class EffectFollowGO : MonoBehaviour
    {
        private Transform _followTarget;
        
        public void Initialize(Transform target)
        {
            _followTarget = target;
        }

        private void LateUpdate()
        {
            // Follow the target’s position & rotation
            transform.position = _followTarget.position;
            transform.rotation = Quaternion.identity;
        }
    }

}
