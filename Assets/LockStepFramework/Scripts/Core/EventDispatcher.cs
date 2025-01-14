using System;
using System.Collections.Generic;

public delegate void EventDispatcherHandler<T>(string evtName, params T[] args);

public class EventDispatcher<T> : Singleton<EventDispatcher<T>>
{
    // Event Name, function
    private Dictionary<string, EventDispatcherHandler<T>> mEventMap = null;

    protected Dictionary<string, EventDispatcherHandler<T>> Events
    {
        get
        {
            if (mEventMap == null)
                mEventMap = new Dictionary<string, EventDispatcherHandler<T>>();
            return mEventMap;
        }
    }

    public void AddEvent(string evtName, EventDispatcherHandler<T> evt)
    {
        if (Events.TryGetValue(evtName, out var evts))
        {
            if (!IsEventRegistered(evts, evt))
            {
                evts += evt;
                mEventMap[evtName] = evts;
            }
        }
        else
        {
            Events.Add(evtName, evt);
        }
    }

    private bool IsEventRegistered(EventDispatcherHandler<T> act, Delegate prospectiveHandler)
    {
        if (act == null)
            return false;
        Delegate[] invoList = act.GetInvocationList();
        foreach (var handler in invoList)
        {
            if (prospectiveHandler == handler)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveEventByName(string evtName)
    {
        if (mEventMap == null)
            return;

        if (mEventMap.TryGetValue(evtName, out var evts))
        {
            if (evts != null)
            {
                mEventMap.Remove(evtName);
            }
            else
            {
                throw new Exception("EventDispatch evt type is null");
            }
        }
    }

    public void ClearAllEvents()
    {
        mEventMap?.Clear();
    }

    public void RemoveEvent(string evtName, EventDispatcherHandler<T> evt)
    {
        if (mEventMap == null || evt == null)
            return;

        if (mEventMap.TryGetValue(evtName, out var evts))
        {
            if (evts != null)
            {
                evts -= evt;
                if (evts == null)
                    mEventMap.Remove(evtName);
                else
                    mEventMap[evtName] = evts;
            }
            else
            {
                throw new Exception("EventDispatch evt type is null");
            }
        }
    }
    
    public void TriggerEvent(string evtName, params T[] args)
    {
        if (mEventMap == null)
            return;

        if (mEventMap.TryGetValue(evtName, out var evts))
        {
            foreach (var act in evts.GetInvocationList())
            {
                if (act is EventDispatcherHandler<T> handler)
                    handler(evtName, args);
            }
        }
    }
}
