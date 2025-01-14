

using System;
using System.Collections.Generic;

namespace Battle
{
    public class TimeMgr : Singleton<TimeMgr>
    {
        private BinaryHeap<Fix64> timeHeap;
        private SortedDictionary<Fix64, Action> timeAction;
        private Fix64 time;

        public void Init()
        {
            time = Fix64.Zero;
            timeHeap = new BinaryHeap<Fix64>(5000);
            timeAction = new SortedDictionary<Fix64, Action>();
        }

        //
        public void LockStepUpdate()
        {
            time += GameData.instance._FixFrameLen;
            DoTrigger();
        }

        private void DoTrigger()
        {
            if (timeHeap.Count > 0 && timeHeap.GetRootValue() <= time)
            {
                Fix64 timeStamp = timeHeap.Pop();
                if (timeAction.TryGetValue(timeStamp, out Action action))
                {
                    action.Invoke();
                    timeAction.Remove(timeStamp);
                }
                DoTrigger();
            }
        }

        public void SetTimeAction(Fix64 nextTime, Action action)
        {
            Fix64 t = time + nextTime;

            if (timeAction.ContainsKey(t))
            {
                timeAction[t] += action;
            }
            else
            {
                Action myAction = null;
                myAction += action;
                timeAction.Add(t, myAction);
                timeHeap.Push(t);
            }
        }

        public void Release()
        {
            time = Fix64.Zero;
            timeHeap.Clear();
            timeAction.Clear();
        }
    }

}