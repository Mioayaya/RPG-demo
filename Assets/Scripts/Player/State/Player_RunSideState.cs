using UnityEngine;

public class Player_RunSideState : PlayerStateBase
{
    public override void Enter()
    {
        player.PlayAnimation("RunSide");
        player.Model.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ÇÐ»»µ½ÌøÔ¾×´Ì¬
            jumpPower =  2;
            player.ChangeState(PlayerState.Jump);
            return;
        }

        if (CheekAnimatorStateName("RunSide", out float animationTime))
        {
            if (animationTime > 0.8f)
            {
                player.ChangeState(PlayerState.Move);
            }
        }
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}
