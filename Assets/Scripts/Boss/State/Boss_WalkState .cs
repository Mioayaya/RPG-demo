using UnityEngine;
using UnityEngine.UIElements;

public class Boss_WalkState : BossStateBase
{
    private LayerMask groundLayerMask = LayerMask.GetMask("Env");
    public override void Enter()
    {
        boss.PlayAnimation("Walk");
        boss.navMeshAgent.enabled = true;
        boss.navMeshAgent.speed = boss.walkSpeed;
    }

    public override void Update()
    {
        float distance = Vector3.Distance(boss.transform.position, boss.targetPlayer.transform.position);
        if(distance <= boss.standAttackRang)
        {
            boss.ChangeState(BossState.Attack);
            return;
        }
        else if (distance >= boss.walkRang)
        {
            boss.ChangeState(BossState.Run);
            return;
        }else
        {
            boss.navMeshAgent.SetDestination(boss.targetPlayer.transform.position);
        }
    }

    public override void Exit()
    {
        boss.navMeshAgent.enabled = false;
    }


}
