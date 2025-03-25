using UnityEngine;

public class Boss_AttackState : BossStateBase
{
    // 索敌平滑转向参数
    private float rotationSpeed = 720f; // 度/秒
    // 当前是第几段普攻
    private int currentAttackIndex;
    private int CurrentAttackIndex
    {
        get => currentAttackIndex;
        set
        {
            if (value >= boss.standAttackConfigs.Length) currentAttackIndex = 0;
            else currentAttackIndex = value;
        }
    }
    public override void Enter()
    {
        // 注册根运动
        //boss.Model.SetRootMotionAction(OnRootMotion);
        CurrentAttackIndex = -1;
        // 播放技能
        StandAttack();
    }

    public override void Update()
    {
        // 持续平滑转向玩家
        RotateTowardsPlayer();      
        float distance = Vector3.Distance(boss.transform.position, boss.targetPlayer.transform.position);
        
        if (distance <= boss.standAttackRang && boss.CanSwitchSkill)
        {
            //boss.ChangeState(BossState.Attack);
            StandAttack();
            return;
        }

        // 待机检测
        bool ret = CheekAnimatorStateName(boss.standAttackConfigs[CurrentAttackIndex].AnimationName, out float animationTime);
        if (ret && boss.CanSwitchSkill)
        {
            // 回到待机
            boss.ChangeState(BossState.Idle);
            return;
        }
    }

    public override void Exit()
    {
        boss.OnSkillOver();
    }

    private void StandAttack()
    {
        //boss.transform.LookAt(boss.targetPlayer.transform);
        boss.transform.LookAt(boss.attackTarget);
        CurrentAttackIndex += 1;
        boss.StartAttack(boss.standAttackConfigs[CurrentAttackIndex]);
    }

    private void RotateTowardsPlayer()
    {
        if (boss.attackTarget == null) return;
        Vector3 direction = (boss.attackTarget.position - boss.transform.position).normalized;
        direction.y = 0; // 保持水平旋转
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            boss.transform.rotation = Quaternion.RotateTowards(
                boss.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = boss.gravity * Time.deltaTime;
        boss.CharacterController.Move(deltaPosition);
    }
}
