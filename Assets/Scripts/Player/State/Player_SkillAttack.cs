using UnityEngine;

public class Player_SkillAttack: PlayerStateBase
{
    private SkillConfig skillConfig;
    public override void Enter()
    {
        // 注册根运动
        //player.Model.SetRootMotionAction(OnRootMotion);
    }

    public void InitData(SkillConfig skillConfig)
    {
        this.skillConfig = skillConfig;
        StartSkill();
    }

    public override void Update()
    {
        Debug.Log("1");

        // 待机检测
        bool ret = CheekAnimatorStateName(skillConfig.AnimationName, out float animationTime);
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
                return;
            }

            // 技能的再次检测
            if (player.CheckAndEnterSkillState()) return;

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
        skillConfig = null;
        player.OnSkillOver();
    }

    private void StartSkill()
    {
        player.StartAttack(skillConfig);
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}


