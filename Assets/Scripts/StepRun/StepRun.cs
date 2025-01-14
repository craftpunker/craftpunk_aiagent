using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StepRun
{
    static Queue<IStep> __StepQueue;
    static Queue<IStep> _StepQueue { 
        get
        {
            if (__StepQueue == null) {
                __StepQueue = new Queue<IStep>();
            }
            return __StepQueue;
        } 
    }
    static readonly int _RunCount = 5;

    public static void AddStep(IStep step)
    {
        _StepQueue.Enqueue(step);
    }

    public static void Update()
    {
        int count = _StepQueue.Count;
        if (count > 0) {
            for (int i = 0; i < Mathf.Min(count, _RunCount); i++)
            {
                IStep step = _StepQueue.Dequeue();
                step.RunStep();
            }
        }
    }
}
