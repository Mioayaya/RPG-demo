using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 拥有Statemachine的类
public interface IStateMachinerOwner { }
public class StateMachine
{
    private IStateMachinerOwner owner;
    private Dictionary<Type, StateBase> stateDi = new Dictionary<Type, StateBase>();
    public StateBase currentState;    

    public Type CurrentStateType { get => currentState.GetType(); }
    public bool HasState { get => currentState != null; }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(IStateMachinerOwner owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <typeparam name="T">切换的状态类型</typeparam>
    /// <param name="reCurrstate">如果状态没变，是否要刷新状态</param>
    /// <returns></returns>
    public bool ChangeState<T>(bool reCurrstate = false) where T:StateBase,new()
    {
        if (HasState && CurrentStateType == typeof(T) && !reCurrstate) return false;

        // 退出当前状态
        if(HasState)
        {
            currentState.Exit();
            MonoManager.Instance.RemoveUpdateListener(currentState.Update);
            MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
            MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        }

        // 进入新状态
        currentState = GetState<T>();
        currentState.Enter();
        MonoManager.Instance.AddUpdateListener(currentState.Update);
        MonoManager.Instance.AddLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.AddFixedUpdateListener(currentState.FixedUpdate);

        return false;
    }

    private StateBase GetState<T>() where T: StateBase,new()
    {
        Type type = typeof(T);
        if(!stateDi.TryGetValue(type, out StateBase state))
        {
            state = new T();
            state.Init(owner);
            stateDi.Add(type, state);
        }
        return state;
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void Stop()
    {
        currentState.Exit();
        MonoManager.Instance.RemoveUpdateListener(currentState.Update);
        MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        currentState = null;

        foreach(var item in stateDi.Values)
        {
            item.UnInit();
        }
        stateDi.Clear();
    }
}
