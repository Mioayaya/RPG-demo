using System.Collections;
using UnityEngine;

public class Boss_HurtState : BossStateBase
{
    private Skill_HitData hitData => boss.hitData;
    private ISkillOwner hitSource => boss.hurtSource;

    private float currentHardTime = 0;
    private Coroutine repelCoroutine;
    
    enum HurtChildState
    {
        // 普通受伤
        NormalHurt,
        // 击倒-Down
        Down,
        // 击倒-Rise
        Rise
    }
    private HurtChildState hurtState;
    private HurtChildState HurtState { 
        get => hurtState; 
        set
        {
            hurtState = value;
            switch(hurtState)
            {
                case HurtChildState.NormalHurt:
                    boss.PlayAnimation("Hurt");
                    break;
                case HurtChildState.Down:
                    boss.transform.LookAt(hitSource.ModelTransForm);
                    boss.PlayAnimation("Down");
                    break;
                case HurtChildState.Rise:
                    boss.PlayAnimation("Rise");
                    break;
            }
        } 
    }
    public override void Enter()
    {
        currentHardTime = 0;
        if(hitData.Down) HurtState = HurtChildState.Down;
        else HurtState = HurtChildState.NormalHurt;

        // 击退距离
        if(hitData.RepelVelocity != Vector3.zero)
        {
            repelCoroutine = MonoManager.Instance.StartCoroutine(DoRepel(hitData.RepelTime,hitData.RepelVelocity));
        }
    }

    public override void Update()
    {
        // 没有击飞 击退
        if(repelCoroutine == null)
        {
            boss.CharacterController.Move(new Vector3(0, boss.gravity * Time.deltaTime, 0));
        }
        currentHardTime += Time.deltaTime;
        switch (HurtState)
        {
            case HurtChildState.NormalHurt:
                // 硬直检测 硬直时间到了，并且击飞 击退时间也到了
                if (currentHardTime >= hitData.HardTime && repelCoroutine == null) boss.ChangeState(BossState.Idle);
                break;
            case HurtChildState.Down:
                //boss.ModelTransForm.LookAt(hitSource.ModelTransForm);
                if (currentHardTime >= hitData.HardTime && repelCoroutine == null) HurtState = HurtChildState.Rise;
                break;
            case HurtChildState.Rise:
                // 检测起身
                if(CheekAnimatorStateName("Rise",out float time) && time >= 0.99f)
                {
                    boss.ChangeState(BossState.Idle);
                }
                break;
        }
    }

    public override void Exit()
    {
        if(repelCoroutine != null)
        {
            MonoManager.Instance.StopCoroutine(repelCoroutine);
            repelCoroutine = null;
        }
    }

    private IEnumerator DoRepel(float time, Vector3 velocity)
    {
        float currentTime = 0;

        // 避免分母为0
        time = time == 0 ? 0.01f : time;
        // 将位移方向修改为针对对手的方向
        Vector3 veloFromSource = hitSource.ModelTransForm.TransformDirection(velocity);

        while(currentTime < time)
        {
            boss.CharacterController.Move(veloFromSource / time * Time.deltaTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        repelCoroutine = null;
    }
}
