using System;
using System.Collections.Generic;

public class GameEvent { }

// A simple Event System that can be used for remote systems communication
public class EventManager
{
    static readonly Dictionary<Type, Action<GameEvent>> SEvents = new Dictionary<Type, Action<GameEvent>>();
    static readonly Dictionary<Delegate, Action<GameEvent>> SEventLookups = new Dictionary<Delegate, Action<GameEvent>>();

    public static void AddListener<T>(Action<T> evt) where T : GameEvent
    {
        if (!SEventLookups.ContainsKey(evt))
        {
            Action<GameEvent> newAction = e => evt((T)e);
            SEventLookups[evt] = newAction;

            if (SEvents.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
                SEvents[typeof(T)] = internalAction += newAction;
            else
                SEvents[typeof(T)] = newAction;
        }
    }

    public static void RemoveListener<T>(Action<T> evt) where T : GameEvent
    {
        if (SEventLookups.TryGetValue(evt, out Action<GameEvent> action))
        {
            if (SEvents.TryGetValue(typeof(T), out Action<GameEvent> tempAction))
            {
                tempAction -= action;
                if (tempAction == null)
                    SEvents.Remove(typeof(T));
                else
                    SEvents[typeof(T)] = tempAction;
            }

            SEventLookups.Remove(evt);
        }
    }

    public static void Broadcast(GameEvent evt)
    {
        if (SEvents.TryGetValue(evt.GetType(), out Action<GameEvent> action))
            action.Invoke(evt);
    }

    public static void Clear()
    {
        SEvents.Clear();
        SEventLookups.Clear();
    }
}
