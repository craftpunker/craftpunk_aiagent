
#if _CLIENTLOGIC_
using System;
using System.Collections.Generic;
using UnityEngine;
using Battle;

public class MonoTimeMgr : Singleton<MonoTimeMgr>
{
    private BinaryHeap<float> timeHeap;
    private Dictionary<float, Action> timeAction;
    private float time;

    public void Init()
    {
        time = 0;
        timeHeap = new BinaryHeap<float>(20);
        timeAction = new Dictionary<float, Action>();
    }

    //
    public void Update(float deltaTime)
    {
        time += deltaTime;
        DoTrigger();
    }

    private void DoTrigger()
    {
        if (timeHeap.Count > 0 && timeHeap.GetRootValue() <= time)
        {
            float timeStamp = timeHeap.Pop();
            if (timeAction.TryGetValue(timeStamp, out Action action))
            {
                action.Invoke();
                timeAction.Remove(timeStamp);
            }
            DoTrigger();
        }
    }

    public void SetTimeAction(float nextTime, Action action)
    {
        float t = time + nextTime;

        if (timeAction.ContainsKey(t))
        {
            timeAction[t] += action;
        }
        else
        {
            Action myAction = null ;
            myAction += action;
            timeAction.Add(t, myAction);
            timeHeap.Push(t);
        }
    }
}
#endif