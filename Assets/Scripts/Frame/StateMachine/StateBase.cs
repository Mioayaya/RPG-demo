using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态基类
/// </summary>
public abstract class StateBase
{
    /// <summary>
    /// 初始化
    /// 只在状态第一次创建的时候调用
    /// </summary>
    /// <param name="owner">宿主</param>
    /// <param name="stateType">当前这个状态代表的实际枚举的int值</param>
    public virtual void Init(IStateMachinerOwner owner){ }

    // 反初始化
    public virtual void UnInit() { }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }
}
