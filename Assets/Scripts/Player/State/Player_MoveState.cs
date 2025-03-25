using UnityEngine;

public class Player_MoveState: PlayerStateBase
{
    private Vector3 lastInput;
    private enum MoveChildeState
    {
        Move,
        Stop
    }

    private enum WalkState
    {
        Walk,Run
    }

    private float walk2RunTransition = 0;  // 0~1
    //private bool walkState = true; // 默认走路
    private WalkState walkState = WalkState.Run;
    private MoveChildeState moveState;
    private MoveChildeState MoveState
    {
        get => moveState;
        set
        {
            moveState = value;
            switch(moveState)
            {
                case MoveChildeState.Move:
                    player.PlayAnimation("Move");
                    // 注册根运动
                    
                    break;
                case MoveChildeState.Stop:
                    player.PlayAnimation("RunStop");
                    //player.Model.ClearRootMotionAction();
                    break;
            }
        }
    }
    public override void Enter()
    {
        MoveState = MoveChildeState.Move;
        player.Model.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        // 检测技能
        if (player.CheckAndEnterSkillState())
        {
            player.ChangeState(PlayerState.SkillAttack);
            return;
        }


        // 格挡检测 右键
        if (Input.GetMouseButtonDown(1))
        {
            player.ChangeState(PlayerState.Defence);
            return;
        }

        // 检测攻击 左键
        if (Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 立即切换朝向
            if(lastInput != Vector3.zero) player.Model.transform.rotation = Quaternion.LookRotation(lastInput);
            // 切换到跳跃状态
            jumpPower = walk2RunTransition + 1;
            player.ChangeState(PlayerState.Jump);
            return;
        }

        // 检测翻滚
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            player.ChangeState(PlayerState.Roll);
            return;
        }

        // 检测滑铲  在速度达到最大时 允许滑铲
        if (Input.GetKeyDown(KeyCode.C) && walk2RunTransition >= 0.9f)
        {
            player.ChangeState(PlayerState.RunSide);
            return;
        }

        if (player.CharacterController.isGrounded == false)
        {
            player.ChangeState(PlayerState.AirDown);
            return;
        }

        switch (moveState)
        {
            case MoveChildeState.Move:
                MoveOnUpdate();
                break;
            case MoveChildeState.Stop:
                StopOnUpdate();
                break;
        }

        
    }

    private void MoveOnUpdate()
    {
        // 停止输入 切回待机
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h == 0 && v == 0)
        {
            if(walkState == WalkState.Run)
            {
                MoveState = MoveChildeState.Stop;
            }
            else
            {
                player.ChangeState(PlayerState.Idle);
            }
            return;
        }
        else
        {
            // 处理 走路与跑步的切换
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
               walkState =  walkState == WalkState.Walk ? WalkState.Run : WalkState.Walk;
            }

            // 如果是走路，跑步切换到走路
            if (walkState == WalkState.Walk)
            {
                player.beforeState = Player_Controller.BeforeState.Walk;
                walk2RunTransition = Mathf.Clamp(walk2RunTransition - Time.deltaTime * player.walk2RunTransition, 0, 1);
            }
            else
            {
                walk2RunTransition = Mathf.Clamp(walk2RunTransition + Time.deltaTime * player.walk2RunTransition, 0, 1);
            }

            if(walk2RunTransition >= 0.99) player.beforeState = Player_Controller.BeforeState.Run; //过度到0.99可以认为是奔跑状态了

            //player.Model.Animator.SetFloat("Move", walk2RunTransition);

            // 此时的速度是影响动画播放速度来达到实际的位移距离变化
            // 仅Idle、Move 状态切到Run 需要渐变，如果之前已经是Run状态，就不需要再次渐变

            if (player.beforeState == Player_Controller.BeforeState.Run)
            {
                player.Model.Animator.SetFloat("Move", player.runSpeed);
            }
            else
            {
                player.Model.Animator.SetFloat("Move", walk2RunTransition);
            }
            player.Model.Animator.speed = Mathf.Lerp(player.walkSpeed, player.runSpeed, walk2RunTransition);

            // 旋转 
            Vector3 input = new Vector3(h, 0, v);
            float y = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
            lastInput = targetDir;
            player.Model.transform.rotation = Quaternion.Slerp(
                player.Model.transform.rotation, 
                Quaternion.LookRotation(targetDir),                 
                Time.deltaTime * player.rotateSpeed);

        }
    }

    private void StopOnUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            // 切换到移动状态
            MoveState = MoveChildeState.Move;
            return;
        }
        // 检测当前动画进度，如果播放完毕了，就切换到待机
        if (CheekAnimatorStateName("RunStop",out float animationTime))
        {
            if (animationTime >= 1) player.ChangeState(PlayerState.Idle);
        }
    }
    public override void Exit()
    {
        walk2RunTransition = 0;
        player.Model.ClearRootMotionAction();
        player.Model.Animator.speed = 1;

    }

    private void OnRootMotion(Vector3 deltaPosition,Quaternion deltaRotation)
    {        
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}