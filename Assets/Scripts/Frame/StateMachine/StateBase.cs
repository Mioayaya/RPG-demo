using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ״̬����
/// </summary>
public abstract class StateBase
{
    /// <summary>
    /// ��ʼ��
    /// ֻ��״̬��һ�δ�����ʱ�����
    /// </summary>
    /// <param name="owner">����</param>
    /// <param name="stateType">��ǰ���״̬�����ʵ��ö�ٵ�intֵ</param>
    public virtual void Init(IStateMachinerOwner owner){ }

    // ����ʼ��
    public virtual void UnInit() { }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }
}
