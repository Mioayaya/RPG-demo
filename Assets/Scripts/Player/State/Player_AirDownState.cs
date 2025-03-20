using UnityEngine;

public class Player_AirDownState : PlayerStateBase
{
    private enum AirDownChildStae
    {
        Loop,
        End
    }

    private float playEndAnimationHeight = 3.5f; // End�����Ƿ񲥷�
    private float endAnimationHeight = 2f;  // End�������Ÿ߶�
    private float runSideHeight = 1f; // ��������߶�
    private bool needEndAniamtion;
    private bool isRunSide = false;
    private LayerMask groundLayerMask = LayerMask.GetMask("Env");
    private AirDownChildStae airDownState;
    private AirDownChildStae AirDownState
    {
        get => airDownState;
        set
        {
            airDownState = value;
            switch(airDownState)
            {
                case AirDownChildStae.Loop:
                    player.PlayAnimation("JumpLoop");
                    break;
                case AirDownChildStae.End:
                    player.PlayAnimation("JumpEnd");
                    break;

            }
        }
    }
    public override void Enter()
    {
        isRunSide = false;
        AirDownState = AirDownChildStae.Loop;
        // �жϵ�ǰ��ɫ�ĸ߶��Ƿ��л���End
        needEndAniamtion = !Physics.Raycast(player.transform.position + new Vector3(0, 0.5f, 0), player.transform.up * -1, playEndAnimationHeight + 0.5f, groundLayerMask);

    }

    public override void Update()
    {
        switch (airDownState)
        {
            case AirDownChildStae.Loop:

                // ��⻬��
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Physics.Raycast(
                            player.transform.position + new Vector3(0, 0.5f, 0),
                            Vector3.down,
                            out RaycastHit hitInfo,
                            runSideHeight + 0.5f,
                            groundLayerMask);
                    // ����ʵ�ʸ߶ȣ�������㵽��ײ��ľ����ȥ���Y��ƫ����
                    float actualHeight = hitInfo.distance - 0.5f;
                    if(actualHeight < runSideHeight) isRunSide = true;
                }

                if (needEndAniamtion)
                {
                    // �߶ȼ��
                    if (Physics.Raycast(player.transform.position + new Vector3(0, 0.5f, 0), player.transform.up * -1, endAnimationHeight + 0.5f, groundLayerMask))
                    {
                        AirDownState = AirDownChildStae.End;
                    }
                }
                else
                {
                    if (player.CharacterController.isGrounded)
                    {
                        // ��⻬��
                        if (isRunSide)
                        {
                            player.ChangeState(PlayerState.RunSide);
                            return;
                        }
                        else
                        {
                            player.OnFootStep();
                            player.ChangeState(PlayerState.Idle);
                            return;
                        }
                    }
                }
                AirControll();
                break;
            case AirDownChildStae.End:
                if(CheekAnimatorStateName("JumpEnd",out float animationTime))
                {
                    if(animationTime >= 0.8f)
                    {
                        // ��ʱ��Ȼ�ȿգ�������׹
                        if(player.CharacterController.isGrounded == false)
                        {
                            AirDownState = AirDownChildStae.Loop;
                        }
                        else
                        {
                            player.ChangeState(PlayerState.Idle);
                        }
                    }
                    else if(animationTime < 0.6f)
                    {
                        AirControll();
                    }
                }
                break;

        }
    }

    private void AirControll()
    {
        // ����λ��
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 motion = new Vector3(0, player.gravity * Time.deltaTime, 0);
        if (h != 0 || v != 0)
        {
            Vector3 input = new Vector3(h, 0, v);
            Vector3 dir = Camera.main.transform.TransformDirection(input);
            motion.x = player.moveSpeedForAirDown * Time.deltaTime * dir.x;
            motion.z = player.moveSpeedForAirDown * Time.deltaTime * dir.z;
            player.CharacterController.Move(player.moveSpeedForAirDown * Time.deltaTime * dir);
            // ��ת
            //float y = Camera.main.transform.rotation.eulerAngles.y;
            //Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
            //player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);

        }
        player.CharacterController.Move(motion);
    }
}
