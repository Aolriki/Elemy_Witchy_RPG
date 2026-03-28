using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public State currState;
    public State previousState;

    public void Set(State newState, bool forceReset = false)
    {
        if (currState != newState || forceReset)
        {
            currState?.ExitStates();
            previousState = currState;
            currState = newState;
            currState?.Initialise(this);
            currState?.Enter();
            currState?.InitializeSubState();
        }
    }

    public List<State> GetActiveStateBranch(List<State> list = null)
    {
        if (list == null)
            list = new List<State>();

        if (currState == null)
            return list;
        else
        {
            list.Add(currState);
            return currState.machine.GetActiveStateBranch(list);
        }
    }


    public State GetRootOfActiveBranch()
    {
       
        if (currState.Parent == null)
        {
            return currState;
        }
        else
        {
            return currState.Parent.GetRootOfActiveBranch();
        }
    }
}
