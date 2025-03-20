using UnityEngine;

public class PlayerStateBase : StateBase
{
    protected Player_Controller player;
    protected static float jumpPower;
    public override void Init(IStateMachinerOwner owner)
    {
        base.Init(owner);
        player = (Player_Controller)owner;
    }

    protected virtual bool CheekAnimatorStateName(string stateName,out float normalizedTime)
    {
        // 处于动画过度阶段，优先判断下一个状态
        AnimatorStateInfo nextInfo = player.Model.Animator.GetNextAnimatorStateInfo(0);
        if(nextInfo.IsName(stateName))
        {
            normalizedTime = nextInfo.normalizedTime;
            return true;
        }
        AnimatorStateInfo currentInfo = player.Model.Animator.GetCurrentAnimatorStateInfo(0);
        normalizedTime = currentInfo.normalizedTime;
        return currentInfo.IsName(stateName);
    }
}