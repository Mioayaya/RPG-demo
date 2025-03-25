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
    //private bool walkState = true; // Ĭ����·
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
                    // ע����˶�
                    
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
        // ��⼼��
        if (player.CheckAndEnterSkillState())
        {
            player.ChangeState(PlayerState.SkillAttack);
            return;
        }


        // �񵲼�� �Ҽ�
        if (Input.GetMouseButtonDown(1))
        {
            player.ChangeState(PlayerState.Defence);
            return;
        }

        // ��⹥�� ���
        if (Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // �����л�����
            if(lastInput != Vector3.zero) player.Model.transform.rotation = Quaternion.LookRotation(lastInput);
            // �л�����Ծ״̬
            jumpPower = walk2RunTransition + 1;
            player.ChangeState(PlayerState.Jump);
            return;
        }

        // ��ⷭ��
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            player.ChangeState(PlayerState.Roll);
            return;
        }

        // ��⻬��  ���ٶȴﵽ���ʱ ������
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
        // ֹͣ���� �лش���
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
            // ���� ��·���ܲ����л�
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
               walkState =  walkState == WalkState.Walk ? WalkState.Run : WalkState.Walk;
            }

            // �������·���ܲ��л�����·
            if (walkState == WalkState.Walk)
            {
                player.beforeState = Player_Controller.BeforeState.Walk;
                walk2RunTransition = Mathf.Clamp(walk2RunTransition - Time.deltaTime * player.walk2RunTransition, 0, 1);
            }
            else
            {
                walk2RunTransition = Mathf.Clamp(walk2RunTransition + Time.deltaTime * player.walk2RunTransition, 0, 1);
            }

            if(walk2RunTransition >= 0.99) player.beforeState = Player_Controller.BeforeState.Run; //���ȵ�0.99������Ϊ�Ǳ���״̬��

            //player.Model.Animator.SetFloat("Move", walk2RunTransition);

            // ��ʱ���ٶ���Ӱ�춯�������ٶ����ﵽʵ�ʵ�λ�ƾ���仯
            // ��Idle��Move ״̬�е�Run ��Ҫ���䣬���֮ǰ�Ѿ���Run״̬���Ͳ���Ҫ�ٴν���

            if (player.beforeState == Player_Controller.BeforeState.Run)
            {
                player.Model.Animator.SetFloat("Move", player.runSpeed);
            }
            else
            {
                player.Model.Animator.SetFloat("Move", walk2RunTransition);
            }
            player.Model.Animator.speed = Mathf.Lerp(player.walkSpeed, player.runSpeed, walk2RunTransition);

            // ��ת 
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
            // �л����ƶ�״̬
            MoveState = MoveChildeState.Move;
            return;
        }
        // ��⵱ǰ�������ȣ������������ˣ����л�������
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