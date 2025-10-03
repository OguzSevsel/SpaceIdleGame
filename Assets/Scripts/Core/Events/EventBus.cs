using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _eventTable = new Dictionary<Type, Delegate>();

    // Subscribe to an event
    public static void Subscribe<T>(Action<T> listener)
    {
        if (_eventTable.TryGetValue(typeof(T), out var del))
        {
            _eventTable[typeof(T)] = Delegate.Combine(del, listener);
        }
        else
        {
            _eventTable[typeof(T)] = listener;
        }
    }

    // Unsubscribe from an event
    public static void Unsubscribe<T>(Action<T> listener)
    {
        if (_eventTable.TryGetValue(typeof(T), out var del))
        {
            var newDel = Delegate.Remove(del, listener);
            if (newDel == null) _eventTable.Remove(typeof(T));
            else _eventTable[typeof(T)] = newDel;
        }
    }

    // Publish (fire) an event
    public static void Publish<T>(T eventData)
    {
        if (_eventTable.TryGetValue(typeof(T), out var del))
        {
            (del as Action<T>)?.Invoke(eventData);
        }
    }

    internal static object Subscribe<T>()
    {
        throw new NotImplementedException();
    }
}
