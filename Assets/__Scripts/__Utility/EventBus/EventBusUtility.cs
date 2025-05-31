using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Course.Utility.Events
{
    /// <summary>
    /// A utility class for managing event buses and event types in the application.
    /// Provides methods for initializing and clearing event buses.
    /// </summary>
    [ExcludeFromCoverage]
    [ExcludeFromCodeCoverage]
    public static class EventBusUtility
    {
        /// <summary>
        /// A read-only list of event types available in the application.
        /// Populated during runtime initialization.
        /// </summary>
        public static IReadOnlyList<Type> EventTypes { get; set; }

        /// <summary>
        /// A read-only list of event bus types corresponding to the event types.
        /// Populated during runtime initialization.
        /// </summary>
        public static IReadOnlyList<Type> EventBusTypes { get; set; }

    #if UNITY_EDITOR
        /// <summary>
        /// Represents the current play mode state in the Unity Editor.
        /// Updated when the play mode state changes.
        /// </summary>
        public static PlayModeStateChange PlayModeState { get; set; }

        /// <summary>
        /// Initializes the editor-specific functionality for the event bus utility.
        /// Subscribes to the play mode state change event.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Handles changes in the play mode state in the Unity Editor.
        /// Clears all event buses when exiting play mode.
        /// </summary>
        /// <param name="state">The new play mode state.</param>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            PlayModeState = state;
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ClearAllBuses();
            }
        }
    #endif

        /// <summary>
        /// Initializes the event bus utility during runtime.
        /// Populates the event types and initializes all event buses.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EventTypes = PredefinedAssemblyUtility.GetTypes(typeof(IEvent));
            EventBusTypes = InitializeAllBuses();
        }

        /// <summary>
        /// Initializes all event buses based on the available event types.
        /// Creates generic event bus types for each event type.
        /// </summary>
        /// <returns>A list of initialized event bus types.</returns>
        private static List<Type> InitializeAllBuses()
        {
            List<Type> eventBusTypes = new List<Type>();

            var typedef = typeof(EventBus<>);
            foreach (var eventType in EventTypes)
            {
                var busType = typedef.MakeGenericType(eventType);
                eventBusTypes.Add(busType);
            }

            return eventBusTypes;
        }

        /// <summary>
        /// Clears all event buses by invoking their internal clear methods.
        /// Ensures that all event buses are reset to their initial state.
        /// </summary>
        public static void ClearAllBuses()
        {
            for (int i = 0; i < EventBusTypes.Count; i++)
            {
                var busType = EventBusTypes[i];
                var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
                clearMethod?.Invoke(null, null);
            }
        }
    }
}
