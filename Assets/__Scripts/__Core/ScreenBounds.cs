using System.Runtime.CompilerServices;
using UnityEngine;
using Utility;

namespace Course.Core
{
    [DefaultExecutionOrder(-1000)]
    [RequireComponent(typeof(BoxCollider))]
    public class ScreenBounds : PrivateSingleton<ScreenBounds>,IScreenBounds
    {
        [Tooltip("World-space Z plane where wrapped objects live (e.g. 0)")]
        [SerializeField] float wrapPlaneZ = 0f;
        [Tooltip("Thickness of the trigger along Z to catch any object moving in front/behind")]
        [SerializeField] float thickness = 5f;
        
        private Camera    _cam;
        private BoxCollider _box;
        private Vector2 _screenMin;
        private Vector2 _screenMax;
        private int     _lastW, _lastH;
        
        #region IScreenBounds API       
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


        public bool OOB(Vector3 worldPos)
        {
            Vector3 locPos = TryGetInstance().transform.InverseTransformPoint(worldPos);
            // Find in which dimension the locPos is furthest from the origin
            float maxDist = Mathf.Max( Mathf.Abs(locPos.x), Mathf.Abs(locPos.y), Mathf.Abs(locPos.z) );
            // If that furthest distance is >0.5f, then worldPos is out of bounds
            return (maxDist > 0.5f);
        }


        public int OOB_X(Vector3 worldPos)
        {
            Vector3 locPos = TryGetInstance().transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.x);
        }
        public int OOB_Y(Vector3 worldPos)
        {
            Vector3 locPos = TryGetInstance().transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.y);
        }
        public int OOB_Z(Vector3 worldPos)
        {
            Vector3 locPos = TryGetInstance().transform.InverseTransformPoint(worldPos);
            return OOB_(locPos.z);
        }


        public int OOB_(float num)
        {
            if (num > 0.5f) return 1;
            if (num < -0.5f) return -1;
            return 0;
        }
        
        #endregion

        #region Private Methods
        protected override void Awake()
        {
            base.Awake();
            _cam  ??= Camera.main;
            if(!_cam.orthographic)
                _cam.orthographic = true;
            _cam.orthographicSize = 9f;
            
            _box  = GetComponent<BoxCollider>();
            _box.isTrigger = true;
            // UpdateScreenBounds();
            ResizeCollider();
        }

        private void Update()
        {
            if (Screen.width != _lastW || Screen.height != _lastH)
            {
                // UpdateScreenBounds();
                ResizeCollider();
            }
        }

        /// <summary>
        ///  AABB collider that matches the screen size
        /// </summary>
        /// <param name="other"></param>
        // private void UpdateScreenBounds()
        // {
        //     _lastW = Screen.width;
        //     _lastH = Screen.height;
        //
        //     var zDist = Mathf.Abs(_cam.transform.position.z - wrapPlaneZ);
        //     var bl = _cam.ScreenToWorldPoint(new Vector3(0, 0, zDist));
        //     var tr = _cam.ScreenToWorldPoint(new Vector3(_lastW, _lastH, zDist));
        //
        //     _screenMin = new Vector2(bl.x, bl.y);
        //     _screenMax = new Vector2(tr.x, tr.y);
        // }
        //
        // private void ResizeCollider()
        // {
        //     var width  = _screenMax.x - _screenMin.x;
        //     var height = _screenMax.y - _screenMin.y;
        //     
        //     transform.position = new Vector3(
        //         (_screenMin.x + _screenMax.x) * 0.5f,
        //         (_screenMin.y + _screenMax.y) * 0.5f,
        //         wrapPlaneZ
        //     );
        //
        //     _box.size = new Vector3(width, height, thickness);
        // }
        
        /// <summary>
        /// Local-space flip
        /// </summary>
        /// <param name="other"></param>
        
        private void ResizeCollider()
        {
            _lastW = Screen.width;
            _lastH = Screen.height;
            
            var height = _cam.orthographicSize * 2f;     
            var width  = height * _cam.aspect;          
        
            transform.position = new Vector3(
                _cam.transform.position.x,
                _cam.transform.position.y,
                wrapPlaneZ
            );
        
            transform.localScale = new Vector3(width, height, 1f);
        
            _box.size = new Vector3(1f, 1f, thickness);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<OffScreenWrapper>(out var wrapper) 
                && wrapper.enabled) 
                wrapper.Wrap(this);
                // wrapper.Wrap(_screenMin, _screenMax);
        }
        #endregion
    }
}
