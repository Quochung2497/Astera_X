using System;
using UnityEngine;

namespace Course.Core
{
    [RequireComponent(typeof(Collider))]
    public class OffScreenWrapper : MonoBehaviour, IOffScreenWrapper
    {
        /// <summary>
        /// Reference to the Collider component attached to the GameObject.
        /// Initialized in the Awake method.
        /// </summary>
        private Collider _collider;

        public event Action<GameObject> OnWrap;
        /// <summary>
        /// Wraps the GameObject's position within the specified screen bounds.
        /// If the position exceeds the bounds, it is flipped and adjusted inward by a small epsilon value.
        /// </summary>
        /// <param name="bounds">The screen bounds within which the GameObject's position is wrapped.</param>
        public void Wrap(ScreenBounds bounds)
        {
            OnWrap?.Invoke(gameObject);
            // Convert the GameObject's position to local space relative to the bounds.
            var local = bounds.transform.InverseTransformPoint(transform.position);

            transform.position = bounds.transform.TransformPoint(ScreenBoundsRules.Wrap(local));
        }

        /// <summary>
        /// Unity's Awake method.
        /// Initializes the Collider component reference.
        /// </summary>
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }
    }
}

