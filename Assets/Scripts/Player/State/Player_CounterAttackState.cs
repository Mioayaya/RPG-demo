using UnityEngine;

public class Player_CounterAttackState:PlayerStateBase
{
    private ISkillOwner hurtSource => player.hurtSource;
    public override void Enter()
    {
        // 注册根运动
        //player.Model.SetRootMotionAction(OnRootMotion);

        // 转向受伤源 
        // todo: 通过锁敌
        player.ModelTransForm.LookAt(hurtSource.ModelTransForm);

        // 播放技能
        player.StartAttack(player.counterAttackConfig);
    }

    public override void Update()
    {

        // 待机检测
        bool ret = CheekAnimatorStateName(player.counterAttackConfig.AnimationName, out float animationTime);
        if (ret && animationTime >= 1)
        {
            // 回到待机
            player.ChangeState(PlayerState.Idle);
            return;
        }

        if (player.CanSwitchSkill)
        {
            // 格挡检测 右键
            if (Input.GetMouseButton(1))
            {
                player.ChangeState(PlayerState.Defence);
                return;
            }

            // 攻击检测
            if (Input.GetMouseButtonDown(0))
            {
                player.ChangeState(PlayerState.StandAttack);
            }

            // 检测翻滚
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                player.ChangeState(PlayerState.Roll);
                return;
            }

            // 移动检测
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (h != 0 || v != 0)
            {
                // 切换到移动状态
                player.ChangeState(PlayerState.Move);
                return;
            }
        }
    }

    public override void Exit()
    {
        player.OnSkillOver();
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}