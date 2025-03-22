using UnityEngine;

public class Player_IdleState: PlayerStateBase
{
    private float currentIdleStayTime = 0;
    public override void Enter()
    {
        player.PlayAnimation("Idle");
        currentIdleStayTime = 0;
    }

    public override void Update()
    {
        // 如果时间大于待机时间，则认为之前是待机状态
        currentIdleStayTime += Time.deltaTime;
        if (currentIdleStayTime >= player.idleStayTime) player.beforeState = Player_Controller.BeforeState.Idle;
        
        // 检测技能
        if(player.CheckAndEnterSkillState())
        {
            player.ChangeState(PlayerState.SkillAttack);
            return;
        }

        // 检测攻击
        if(Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }

        // 检测跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 切换到跳跃状态
            jumpPower = 0;
            player.ChangeState(PlayerState.Jump);
            return;
        }

        // 检测翻滚
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            player.ChangeState(PlayerState.Roll);
            return;
        }

        // 空中检测
        //player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));
        if(player.CharacterController.isGrounded == false)
        {
            player.ChangeState(PlayerState.AirDown);
            return;
        }
        
        // 移动检测
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if(h!=0 || v!=0)
        {
            // 切换到移动状态
            player.ChangeState(PlayerState.Move);
            return;
        }

        // 格挡检测 右键格挡
        if(Input.GetMouseButton(1))
        {
            player.ChangeState(PlayerState.Defence);
            return;
        }
    }
}