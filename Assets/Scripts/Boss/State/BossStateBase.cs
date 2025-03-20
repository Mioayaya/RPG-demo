using UnityEngine;

public class BossStateBase : StateBase
{
    protected Boss_Controller boss;
    public override void Init(IStateMachinerOwner owner)
    {
        base.Init(owner);
        boss = (Boss_Controller)owner;
    }

    protected virtual bool CheekAnimatorStateName(string stateName, out float normalizedTime)
    {
        // ���ڶ������Ƚ׶Σ������ж���һ��״̬
        AnimatorStateInfo nextInfo = boss.Model.Animator.GetNextAnimatorStateInfo(0);
        if (nextInfo.IsName(stateName))
        {
            normalizedTime = nextInfo.normalizedTime;
            return true;
        }
        AnimatorStateInfo currentInfo = boss.Model.Animator.GetCurrentAnimatorStateInfo(0);
        normalizedTime = currentInfo.normalizedTime;
        return currentInfo.IsName(stateName);
    }
}
