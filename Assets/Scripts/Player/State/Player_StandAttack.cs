using UnityEngine;

public class Player_StandAttack: PlayerStateBase
{
    // 当前是第几段普攻
    private int currentAttackIndex;
    private int CurrentAttackIndex
    {
        get => currentAttackIndex;
        set
        {
            if (value >= player.currentStandAttackConfigs.Length) currentAttackIndex = 0;
            else currentAttackIndex = value;
        }
    }
    public override void Enter()
    {
        // 注册根运动
        //player.Model.SetRootMotionAction(OnRootMotion);
        CurrentAttackIndex = -1;
        // 播放技能
        StandAttack();
    }

    public override void Update()
    {

        // 待机检测
        bool ret = CheekAnimatorStateName(player.currentStandAttackConfigs[CurrentAttackIndex].AnimationName, out float animationTime);
        if (ret && animationTime >= 1)
        {
            // 回到待机
            player.ChangeState(PlayerState.Idle);
            return;
        }

        if(player.CanSwitchSkill)
        {
            // 格挡检测 右键
            if (Input.GetMouseButton(1))
            {
                player.ChangeState(PlayerState.Defence);
                return;
            }

            // 检测技能
            if (player.CheckAndEnterSkillState())
            {
                player.ChangeState(PlayerState.SkillAttack);
                return;
            }

            // 攻击检测
            if (Input.GetMouseButtonDown(0))
            {
                StandAttack();
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

    private void StandAttack()
    {
        // todo: 实现连续普攻
        CurrentAttackIndex += 1;
        //player.StartAttack(player.standAttackConfigs[CurrentAttackIndex]);
        player.StartAttack(player.currentStandAttackConfigs[CurrentAttackIndex]);
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}


