using UnityEngine;

public class Player_JumpState : PlayerStateBase
{
    public override void Enter()
    {
        //player.PlayAnimation("JumpStart");
        player.PlayAnimation("JumpStart");
        player.Model.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        if(CheekAnimatorStateName("JumpStart", out float animationTime) && animationTime >= 0.9f)
        {
            player.ChangeState(PlayerState.AirDown);
        }
    }
    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.x = 0;
        deltaPosition.z = 0;
        deltaPosition.y *= player.jumpPower;
        Vector3 offset = jumpPower * Time.deltaTime * player.moveSpeedForJump * player.Model.transform.forward;        
        player.CharacterController.Move(deltaPosition + offset);
    }


}
