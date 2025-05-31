using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.TestTools;

namespace Course.Utility.Events
{
  /// <summary>
  /// Interface for event binding with generic type support.
  /// Provides properties for handling events with and without arguments.
  /// </summary>
  /// <typeparam name="T">The type of the event argument.</typeparam>
  internal interface IEventBiding<T>
  {
    /// <summary>
    /// Gets or sets the action to be invoked when the event occurs with arguments.
    /// </summary>
    public Action<T> OnEvent { get; set; }

    /// <summary>
    /// Gets or sets the action to be invoked when the event occurs without arguments.
    /// </summary>
    public Action OnEventNoArgs { get; set; }
  }

  /// <summary>
  /// Implementation of the <see cref="IEventBiding{T}"/> interface.
  /// Provides methods for adding and removing event handlers.
  /// </summary>
  /// <typeparam name="T">The type of the event argument.</typeparam>
  [ExcludeFromCoverage]
  [ExcludeFromCodeCoverage]
  public class EventBinding<T> : IEventBiding<T>
    where T : IEvent
  {
    /// <summary>
    /// Action to be invoked when the event occurs with arguments.
    /// Initialized to a no-op delegate.
    /// </summary>
    private Action<T> OnEvent = _ => { };

    /// <summary>
    /// Action to be invoked when the event occurs without arguments.
    /// Initialized to a no-op delegate.
    /// </summary>
    private Action OnEventNoArgs = () => { };

    /// <inheritdoc/>
    Action<T> IEventBiding<T>.OnEvent
    {
      get => OnEvent;
      set => OnEvent = value;
    }

    /// <inheritdoc/>
    Action IEventBiding<T>.OnEventNoArgs
    {
      get => OnEventNoArgs;
      set => OnEventNoArgs = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBinding{T}"/> class with an event handler for events with arguments.
    /// </summary>
    /// <param name="onEvent">The action to be invoked when the event occurs with arguments.</param>
    public EventBinding(Action<T> onEvent) => this.OnEvent = onEvent;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBinding{T}"/> class with an event handler for events without arguments.
    /// </summary>
    /// <param name="onEventNoArgs">The action to be invoked when the event occurs without arguments.</param>
    public EventBinding(Action onEventNoArgs) => this.OnEventNoArgs = onEventNoArgs;

    /// <summary>
    /// Adds an event handler for events with arguments.
    /// </summary>
    /// <param name="onEvent">The action to be added.</param>
    public void AddEvent(Action<T> onEvent) => this.OnEvent += onEvent;

    /// <summary>
    /// Removes an event handler for events with arguments.
    /// </summary>
    /// <param name="onEvent">The action to be removed.</param>
    public void RemoveEvent(Action<T> onEvent) => this.OnEvent -= onEvent;

    /// <summary>
    /// Adds an event handler for events without arguments.
    /// </summary>
    /// <param name="onEventNoArgs">The action to be added.</param>
    public void AddEventNoArgs(Action onEventNoArgs) => this.OnEventNoArgs += onEventNoArgs;

    /// <summary>
    /// Removes an event handler for events without arguments.
    /// </summary>
    /// <param name="onEventNoArgs">The action to be removed.</param>
    public void RemoveEventNoArgs(Action onEventNoArgs) => this.OnEventNoArgs -= onEventNoArgs;
  }
}
