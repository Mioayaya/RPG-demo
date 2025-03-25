using UnityEngine;

public class Boss_AttackState : BossStateBase
{
    // ����ƽ��ת�����
    private float rotationSpeed = 720f; // ��/��
    // ��ǰ�ǵڼ����չ�
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
        // ע����˶�
        //boss.Model.SetRootMotionAction(OnRootMotion);
        CurrentAttackIndex = -1;
        // ���ż���
        StandAttack();
    }

    public override void Update()
    {
        // ����ƽ��ת�����
        RotateTowardsPlayer();      
        float distance = Vector3.Distance(boss.transform.position, boss.targetPlayer.transform.position);
        
        if (distance <= boss.standAttackRang && boss.CanSwitchSkill)
        {
            //boss.ChangeState(BossState.Attack);
            StandAttack();
            return;
        }

        // �������
        bool ret = CheekAnimatorStateName(boss.standAttackConfigs[CurrentAttackIndex].AnimationName, out float animationTime);
        if (ret && boss.CanSwitchSkill)
        {
            // �ص�����
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
        direction.y = 0; // ����ˮƽ��ת
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
