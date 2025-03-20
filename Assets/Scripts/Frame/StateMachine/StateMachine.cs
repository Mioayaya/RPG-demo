using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ӵ��Statemachine����
public interface IStateMachinerOwner { }
public class StateMachine
{
    private IStateMachinerOwner owner;
    private Dictionary<Type, StateBase> stateDi = new Dictionary<Type, StateBase>();
    public StateBase currentState;    

    public Type CurrentStateType { get => currentState.GetType(); }
    public bool HasState { get => currentState != null; }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Init(IStateMachinerOwner owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// �л�״̬
    /// </summary>
    /// <typeparam name="T">�л���״̬����</typeparam>
    /// <param name="reCurrstate">���״̬û�䣬�Ƿ�Ҫˢ��״̬</param>
    /// <returns></returns>
    public bool ChangeState<T>(bool reCurrstate = false) where T:StateBase,new()
    {
        if (HasState && CurrentStateType == typeof(T) && !reCurrstate) return false;

        // �˳���ǰ״̬
        if(HasState)
        {
            currentState.Exit();
            MonoManager.Instance.RemoveUpdateListener(currentState.Update);
            MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
            MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        }

        // ������״̬
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
    /// ��Ϸ����
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
