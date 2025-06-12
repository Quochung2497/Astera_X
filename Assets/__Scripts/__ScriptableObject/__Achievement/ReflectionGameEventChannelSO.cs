using System;
using System.Linq;
using System.Reflection;
using Course.Utility.Events;
using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(fileName = "ReflectionGameEventChannelSO", menuName = "Scriptable Objects/ReflectionGameEventChannelSO")]
    public class ReflectionGameEventChannelSO : UnityEngine.ScriptableObject
    {
        [Tooltip("Full type name of the IEvent (including namespace, assembly)")]
        public string EventTypeName;

         private Type       _eventType;
        private Delegate   _handler;    // Action<TEvent>
        private object     _binding;    // EventBinding<TEvent> instance
        private MethodInfo _reg, _dereg;

        private void OnEnable()
        {
            // 1) Resolve the event type
            _eventType = Type.GetType(EventTypeName)
                      ?? AppDomain.CurrentDomain.GetAssemblies()
                          .SelectMany(a => {
                            try { return a.GetTypes(); }
                            catch { return Array.Empty<Type>(); }
                          })
                          .FirstOrDefault(t => t.FullName == EventTypeName);

            if (_eventType == null)
            {
                Debug.LogError($"[ReflectionChannel] Could not find type '{EventTypeName}'");
                return;
            }

            // 2) Grab EventBus<T>.Register/Deregister
            var busType = typeof(EventBus<>).MakeGenericType(_eventType);
            _reg   = busType.GetMethod("Register",   BindingFlags.Public | BindingFlags.Static);
            _dereg = busType.GetMethod("Deregister", BindingFlags.Public | BindingFlags.Static);

            // 3) Build your Raise<TEvent> delegate
            var raiseMethod = typeof(ReflectionGameEventChannelSO)
                .GetMethod(nameof(Raise), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(_eventType);

            var actionType = typeof(Action<>).MakeGenericType(_eventType);
            _handler = Delegate.CreateDelegate(actionType, this, raiseMethod);

            // 4) Create EventBinding<TEvent> with that handler
            var bindingType = typeof(EventBinding<>).MakeGenericType(_eventType);
            _binding = Activator.CreateInstance(bindingType, new object[]{ _handler });

            // 5) Register to the bus
            _reg.Invoke(null, new object[]{ _binding });
        }

        private void OnDisable()
        {
            if (_dereg != null && _binding != null)
                _dereg.Invoke(null, new object[]{ _binding });
        }

        // Called by EventBus<T>.Register whenever an event fires
        protected void Raise<TEvent>(TEvent evt)
            where TEvent : IEvent
        {
            OnRaised?.Invoke(evt);
        }

        public event Action<IEvent> OnRaised;
        public void Subscribe(Action<IEvent> h)   => OnRaised += h;
        public void Unsubscribe(Action<IEvent> h) => OnRaised -= h;
    }
}
