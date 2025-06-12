using System.Collections.Generic;
using System.Linq;

namespace Course.Utility.Events
{
  /// <summary>
  /// A static class that serves as an event bus for managing event bindings and raising events.
  /// </summary>
  /// <typeparam name="T">The type of event that the event bus handles. Must implement the <see cref="IEvent"/> interface.</typeparam>
  public static class EventBus<T>
    where T : IEvent
  {
    /// <summary>
    /// A collection of event bindings registered to the event bus.
    /// </summary>
    private static readonly HashSet<IEventBiding<T>> bindings = new HashSet<IEventBiding<T>>();
    
    /// <summary>
    /// A reusable “snapshot” used to hold a copy of the current bindings each time we raise.
    /// </summary>

    /// <summary>
    /// Registers a new event binding to the event bus.
    /// </summary>
    /// <param name="binding">The event binding to register.</param>
    public static void Register(EventBinding<T> binding) => bindings.Add(binding);

    /// <summary>
    /// Deregisters an existing event binding from the event bus.
    /// </summary>
    /// <param name="binding">The event binding to deregister.</param>
    public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

    /// <summary>
    /// Deregisters all event bindings from the event bus.
    /// </summary>
    public static void DeregisterAll() => bindings.Clear();

    /// <summary>
    /// Raises an event, invoking all registered bindings.
    /// </summary>
    /// <param name="event">The event to raise.</param>
    public static void Raise(T @event)
    {
      var snapshot = bindings.ToArray();

      foreach (var b in snapshot)
      {
        b.OnEvent.Invoke(@event);
        b.OnEventNoArgs.Invoke();
      }
    }
  }
}
