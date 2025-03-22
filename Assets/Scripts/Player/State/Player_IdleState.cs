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
        // ���ʱ����ڴ���ʱ�䣬����Ϊ֮ǰ�Ǵ���״̬
        currentIdleStayTime += Time.deltaTime;
        if (currentIdleStayTime >= player.idleStayTime) player.beforeState = Player_Controller.BeforeState.Idle;
        
        // ��⼼��
        if(player.CheckAndEnterSkillState())
        {
            player.ChangeState(PlayerState.SkillAttack);
            return;
        }

        // ��⹥��
        if(Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }

        // �����Ծ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // �л�����Ծ״̬
            jumpPower = 0;
            player.ChangeState(PlayerState.Jump);
            return;
        }

        // ��ⷭ��
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            player.ChangeState(PlayerState.Roll);
            return;
        }

        // ���м��
        //player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));
        if(player.CharacterController.isGrounded == false)
        {
            player.ChangeState(PlayerState.AirDown);
            return;
        }
        
        // �ƶ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if(h!=0 || v!=0)
        {
            // �л����ƶ�״̬
            player.ChangeState(PlayerState.Move);
            return;
        }

        // �񵲼�� �Ҽ���
        if(Input.GetMouseButton(1))
        {
            player.ChangeState(PlayerState.Defence);
            return;
        }
    }
}