using System.Collections;
using Unity;
using UnityEngine;

public class Player_RollState: PlayerStateBase
{
    public override void Enter()
    {
        player.PlayAnimation("Roll");
        player.Model.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        if(CheekAnimatorStateName("Roll",out float animationTime))
        {
            if(animationTime > 0.8f)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}

