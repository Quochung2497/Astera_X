using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.TestTools;

namespace Utility
{
  /// <summary>
  /// A generic singleton class for managing a single instance of a MonoBehaviour-derived component.
  /// Ensures that only one instance of the specified type exists in the scene.
  /// </summary>
  /// <typeparam name="T">The type of the component to be managed as a singleton.</typeparam>
  [ExcludeFromCoverage]
  [ExcludeFromCodeCoverage]
  public class Singleton<T> : MonoBehaviour
    where T : Component
  {
    /// <summary>
    /// The static instance of the singleton.
    /// </summary>
    protected static T instance;

    /// <summary>
    /// Indicates whether the singleton instance exists.
    /// </summary>
    public static bool HasInstance => instance is not null;

    /// <summary>
    /// Attempts to retrieve the singleton instance.
    /// Returns null if the instance does not exist.
    /// </summary>
    /// <returns>The singleton instance or null if it does not exist.</returns>
    public static T TryGetInstance() => HasInstance ? instance : null;

    /// <summary>
    /// Gets the current singleton instance.
    /// </summary>
    public static T Current => instance;

    /// <summary>
    /// Gets or creates the singleton instance.
    /// If no instance exists, it attempts to find one in the scene or creates a new GameObject with the component attached.
    /// </summary>
    public static T Instance
    {
      get
      {
        if (instance)
          return instance;

        // Attempt to find the first object of the specified type in the scene.
        instance = FindFirstObjectByType<T>();
        if (instance)
          return instance;

        // Create a new GameObject and attach the component if no instance is found.
        var obj = new GameObject { name = typeof(T).Name + "AutoCreated" };
        instance = obj.AddComponent<T>();

        return instance;
      }
    }

    /// <summary>
    /// Unity's Awake method, called when the script instance is being loaded.
    /// Initializes the singleton instance.
    /// </summary>
    protected virtual void Awake() => InitializeSingleton();

    /// <summary>
    /// Initializes the singleton instance.
    /// Ensures that the instance is set only during runtime.
    /// </summary>
    protected virtual void InitializeSingleton()
    {
      if (!Application.isPlaying)
      {
        return;
      }

      instance = this as T;
    }
  }
}
