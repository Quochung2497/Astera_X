using System.Runtime.CompilerServices;
using Course.Utility;
using UnityEngine;
using Utility;

namespace Course.Core
{
    [DefaultExecutionOrder(-1000)]
    [RequireComponent(typeof(BoxCollider))]
    public class ScreenBounds : PrivateSingleton<ScreenBounds>,IScreenBounds
    {
        #region Serialize Fields

        [Tooltip("World-space Z plane where wrapped objects live (e.g. 0)")]
        [SerializeField] float wrapPlaneZ = 0f;
        [Tooltip("Thickness of the trigger along Z to catch any object moving in front/behind")]
        [SerializeField] float thickness = 5f;

        #endregion

        #region Private Fields

        /// <summary>
        /// Reference to the main camera in the scene.
        /// Initialized in the Awake method.
        /// </summary>
        private Camera _cam;

        /// <summary>
        /// Reference to the BoxCollider component attached to the GameObject.
        /// Initialized in the Awake method.
        /// </summary>
        private BoxCollider _box;

        /// <summary>
        /// Minimum screen bounds in world space.
        /// </summary>
        private Vector2 _screenMin;

        /// <summary>
        /// Maximum screen bounds in world space.
        /// </summary>
        private Vector2 _screenMax;

        /// <summary>
        /// Stores the last screen width to detect changes.
        /// </summary>
        private int _lastW;

        /// <summary>
        /// Stores the last screen height to detect changes.
        /// </summary>
        private int _lastH;

        #endregion
        
        #region IScreenBounds API

        /// <summary>
        /// Gets a random location within the screen bounds.
        /// </summary>
        public Vector3 RANDOM_ON_SCREEN_LOC
        {
            get
            {
                var inst = TryGetInstance();
                var min = inst._box.bounds.min;
                var max = inst._box.bounds.max;
                var loc = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
                return loc;
            }
        }

        /// <summary>
        /// Gets the bounds of the screen in world space.
        /// </summary>
        public Bounds BOUNDS
        {
            get
            {
                if (TryGetInstance() == null)
                {
                    Debug.LogError("ScreenBounds.BOUNDS - ScreenBounds.S is null!");
                    return new Bounds();
                }
                if (TryGetInstance()._box == null)
                {
                    Debug.LogError("ScreenBounds.BOUNDS - ScreenBounds.S.boxColl is null!");
                    return new Bounds();
                }
                return TryGetInstance()._box.bounds;
            }
        }

        /// <summary>
        /// Checks if a given world position is out of bounds.
        /// </summary>
        /// <param name="worldPos">The world position to check.</param>
        /// <returns>True if the position is out of bounds; otherwise, false.</returns>
        public bool OOB(Vector3 worldPos)
        {
            Vector3 locPos = TryGetInstance().transform.InverseTransformPoint(worldPos);
            float maxDist = Mathf.Max(Mathf.Abs(locPos.x), Mathf.Abs(locPos.y), Mathf.Abs(locPos.z));
            return (maxDist > 0.5f);
        }

        /// <summary>
        /// Checks if a given world position is out of bounds along the X-axis.
        /// </summary>
        /// <param name="worldPos">The world position to check.</param>
        /// <returns>1 if out of bounds positively, -1 if negatively, 0 if within bounds.</returns>
        public int OOB_X(Vector3 worldPos)
        {
            Vector3 locPos = TryGetInstance().transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.x);
        }

        /// <summary>
        /// Checks if a given world position is out of bounds along the Y-axis.
        /// </summary>
        /// <param name="worldPos">The world position to check.</param>
        /// <returns>1 if out of bounds positively, -1 if negatively, 0 if within bounds.</returns>
        public int OOB_Y(Vector3 worldPos)
        {
            Vector3 locPos = TryGetInstance().transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.y);
        }

        /// <summary>
        /// Checks if a given world position is out of bounds along the Z-axis.
        /// </summary>
        /// <param name="worldPos">The world position to check.</param>
        /// <returns>1 if out of bounds positively, -1 if negatively, 0 if within bounds.</returns>
        public int OOB_Z(Vector3 worldPos)
        {
            Vector3 locPos = TryGetInstance().transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.z);
        }

        /// <summary>
        /// Helper method to determine out-of-bounds status for a single axis.
        /// </summary>
        /// <param name="num">The axis value to check.</param>
        /// <returns>1 if out of bounds positively, -1 if negatively, 0 if within bounds.</returns>
        public int OOB_(float num)
        {
            if (num > 0.5f) return 1;
            if (num < -0.5f) return -1;
            return 0;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Unity's Awake method.
        /// Initializes the camera and BoxCollider references and resizes the collider.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _cam ??= Camera.main;
            if (!_cam.orthographic)
                _cam.orthographic = true;
            _cam.orthographicSize = 9f;

            _box = GetComponent<BoxCollider>();
            _box.isTrigger = true;
            ResizeCollider();
        }

        /// <summary>
        /// Unity's Update method.
        /// Resizes the collider if the screen dimensions change.
        /// </summary>
        private void Update()
        {
            if (Screen.width != _lastW || Screen.height != _lastH)
            {
                ResizeCollider();
            }
        }

        /// <summary>
        /// Resizes the BoxCollider to match the screen dimensions.
        /// </summary>
        private void ResizeCollider()
        {
            _lastW = Screen.width;
            _lastH = Screen.height;

            var height = _cam.orthographicSize * 2f;
            var width = height * _cam.aspect;

            transform.position = new Vector3(
                _cam.transform.position.x,
                _cam.transform.position.y,
                wrapPlaneZ
            );

            transform.localScale = new Vector3(width, height, 1f);

            _box.size = new Vector3(1f, 1f, thickness);
        }

        /// <summary>
        /// Handles the OnTriggerExit event.
        /// Wraps objects that exit the screen bounds.
        /// </summary>
        /// <param name="other">The collider of the object that exited the bounds.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<OffScreenWrapper>(out var wrapper) 
                && wrapper.enabled)
                wrapper.Wrap(this);
        }

        #endregion
    }
}
