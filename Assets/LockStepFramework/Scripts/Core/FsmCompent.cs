
using System;
using System.Collections.Generic;

public class FsmCompent<T> where T : class
{
    private FsmState<T> currFsmState;
    private T owner;
    //private FsmState<T>[] states;
    private Dictionary<Type, FsmState<T>> stateDict;

    public void CreateFsm(T owner, params FsmState<T>[] states)
    {
        this.owner = owner;
        //this.states = states;

        stateDict = new Dictionary<Type, FsmState<T>>();

        foreach (FsmState<T> state in states)
        {
            stateDict.Add(state.GetType(), state);
            state.OnInit(owner);
        }
    }

    public void OnStart<TFsm>() where TFsm : FsmState<T>
    {
        currFsmState = GetFsmState<TFsm>();
        currFsmState.OnEnter(owner);
    }

    public void OnUpdate(T owner)
    {
        if(currFsmState != null)
            currFsmState.OnUpdate(owner);
    }

    public void ChangeFsmState<TFsm>() where TFsm : FsmState<T>
    {
        currFsmState.OnLeave(owner);
        var state = GetFsmState<TFsm>();
        currFsmState = state;
        state.OnEnter(owner);
    }

    public FsmState<T> GetFsmState<TFsm>() where TFsm : FsmState<T>
    {
        FsmState<T> state = null;
        stateDict.TryGetValue(typeof(TFsm), out state);

        if (state == null)
        {
            return null;
        }

        return state;
    }

    public FsmState<T> GetCurrState()
    {
        return currFsmState;
    }

    public void ReleaseAllFsmState()
    {
        owner = null;
        currFsmState = null;
        if (stateDict != null)
        {
            foreach (var state in stateDict)
            {
                ClassPool.instance.Push(state.Value);
            }
            stateDict.Clear();
        }

        ClassPool.instance.Push(this);
    }
}


