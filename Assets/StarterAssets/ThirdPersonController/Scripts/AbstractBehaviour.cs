using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractBehaviour<StateType> : MonoBehaviour where StateType : System.Enum
{
    [field: SerializeField]
    public StateType State  //so that no one can set the state directly, they have to call the function
    {
        get; protected set;
    }

    public delegate void StateChangeEvent(StateType OldState, StateType NewState);
    public StateChangeEvent OnStateChange;

    public virtual void ChangeState(StateType NewState)
    {
        OnStateChange?.Invoke(State, NewState);
        State = NewState;
    }
}
