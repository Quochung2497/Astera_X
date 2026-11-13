using Course.Utility.Events;
using UnityEngine;

namespace Course.Core
{
    [DefaultExecutionOrder(-1000)]
    [RequireComponent(typeof(BoxCollider))]
    public class ScreenBounds : MonoBehaviour, IScreenBounds
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
        private float _lastAspect;
        private float _lastOrthographicSize;

        #endregion
        
        #region IScreenBounds API

        /// <summary>
        /// Gets a random location within the screen bounds.
        /// </summary>
        public Vector3 RANDOM_ON_SCREEN_LOC
        {
            get
            {
                var bounds = _box.bounds;
                var min = bounds.min;
                var max = bounds.max;
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
                if (_box == null)
                {
                    Debug.LogError("ScreenBounds.BOUNDS - BoxCollider is not initialized.");
                    return new Bounds();
                }
                return _box.bounds;
            }
        }

        /// <summary>
        /// Checks if a given world position is out of bounds.
        /// </summary>
        /// <param name="worldPos">The world position to check.</param>
        /// <returns>True if the position is out of bounds; otherwise, false.</returns>
        public bool OOB(Vector3 worldPos)
        {
            Vector3 locPos = transform.InverseTransformPoint(worldPos);
            return ScreenBoundsRules.IsOutside(locPos);
        }

        /// <summary>
        /// Checks if a given world position is out of bounds along the X-axis.
        /// </summary>
        /// <param name="worldPos">The world position to check.</param>
        /// <returns>1 if out of bounds positively, -1 if negatively, 0 if within bounds.</returns>
        public int OOB_X(Vector3 worldPos)
        {
            Vector3 locPos = transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.x);
        }

        /// <summary>
        /// Checks if a given world position is out of bounds along the Y-axis.
        /// </summary>
        /// <param name="worldPos">The world position to check.</param>
        /// <returns>1 if out of bounds positively, -1 if negatively, 0 if within bounds.</returns>
        public int OOB_Y(Vector3 worldPos)
        {
            Vector3 locPos = transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.y);
        }

        /// <summary>
        /// Checks if a given world position is out of bounds along the Z-axis.
        /// </summary>
        /// <param name="worldPos">The world position to check.</param>
        /// <returns>1 if out of bounds positively, -1 if negatively, 0 if within bounds.</returns>
        public int OOB_Z(Vector3 worldPos)
        {
            Vector3 locPos = transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.z);
        }

        /// <summary>
        /// Helper method to determine out-of-bounds status for a single axis.
        /// </summary>
        /// <param name="num">The axis value to check.</param>
        /// <returns>1 if out of bounds positively, -1 if negatively, 0 if within bounds.</returns>
        public int OOB_(float num) => ScreenBoundsRules.GetAxisSide(num);

        #endregion

        #region Private Methods

        /// <summary>
        /// Unity's Awake method.
        /// Initializes the camera and BoxCollider references and resizes the collider.
        /// </summary>
        private void Awake()
        {
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
        /// Updates the bounds and notifies listeners when the viewport changes.
        /// </summary>
        private void Update()
        {
            if (Screen.width != _lastW || Screen.height != _lastH ||
                !Mathf.Approximately(_cam.aspect, _lastAspect) ||
                !Mathf.Approximately(_cam.orthographicSize, _lastOrthographicSize))
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
            _lastAspect = _cam.aspect;
            _lastOrthographicSize = _cam.orthographicSize;

            var height = _cam.orthographicSize * 2f;
            var width = height * _cam.aspect;

            transform.position = new Vector3(
                _cam.transform.position.x,
                _cam.transform.position.y,
                wrapPlaneZ
            );

            transform.localScale = new Vector3(width, height, 1f);

            _box.size = new Vector3(1f, 1f, thickness);
            EventBus<ViewportChangedEvent>.Raise(new ViewportChangedEvent(_cam));
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
