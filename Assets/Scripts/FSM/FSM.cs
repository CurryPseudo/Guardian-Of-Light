using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FSM
{
    public enum State
    {
        Null, Wander,//Normal
        EscapeFromLight, CatchLittleMan, HaveCatchedLittleMan,//Monster
        EscapeFromMonster, HaveBeenCatched,//LittleMan
        Normal,Leap,//Player
        WaitPutLight,WaitForUpgrade//PlayerUp
    }
    public enum StateInput
    {
        Enter, Excute, Exit
    }
    public Dictionary<State, Dictionary<StateInput, Action<float>>> rule = new Dictionary<State, Dictionary<StateInput, Action<float>>>();
    public State currentState = State.Null;
    public FSM()
    {
    }
    public void ChangeState(State state)
    {
        if (currentState != State.Null)
        {
            rule[currentState][StateInput.Exit](0);
        }
        currentState = state;
        rule[currentState][StateInput.Enter](0);
    }
    public void Update(float time)
    {
        if (currentState != State.Null)
            rule[currentState][StateInput.Excute](time);
    }
    public void RefreshState()
    {
        if (currentState != State.Null)
        {
            rule[currentState][StateInput.Exit](0);
            rule[currentState][StateInput.Enter](0);
        }
    }
}
    
