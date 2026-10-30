using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Course.Utility
{
    [RequireComponent(typeof(RectTransform))]
    public class VirtualJoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField]
        private RectTransform handle;   
        [SerializeField] 
        private RectTransform bg;   
        
        public Action<Vector2> OnValueChanged; 

        private Vector2 _startPos;
        private float _dragRange;
        private int? _pointerId;

        void Awake()
        {
            _startPos = handle.anchoredPosition;
            float bgHalfWidth  = bg.rect.width  / 2f;
            float bgHalfHeight = bg.rect.height / 2f;
            float bgRadius     = Mathf.Min(bgHalfWidth, bgHalfHeight);
            float handleRadius = Mathf.Max(handle.rect.width, handle.rect.height) / 2f;
            _dragRange = bgRadius - handleRadius;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointerId.HasValue) return;
            _pointerId = eventData.pointerId;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_pointerId != eventData.pointerId) return;

            // Convert screen point to local point in this RectTransform
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint);

            // Clamp to circle of radius dragRange
            Vector2 clamped = Vector2.ClampMagnitude(localPoint, _dragRange);
            handle.anchoredPosition = _startPos + clamped;

            // Send normalized direction
            OnValueChanged?.Invoke(clamped / _dragRange);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerId != eventData.pointerId) return;
            ResetInput();
        }

        private void OnDisable()
        {
            ResetInput();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) ResetInput();
        }

        private void ResetInput()
        {
            _pointerId = null;
            if (handle != null) handle.anchoredPosition = _startPos;
            OnValueChanged?.Invoke(Vector2.zero);
        }
    }
}
