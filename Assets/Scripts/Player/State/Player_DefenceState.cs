using System;
using UnityEngine;

public class Player_DefenceState: PlayerStateBase
{
    // todo: ��ͨ�� �����˺������ǲ����߻���
    // todo: ��׼��(һ��ʱ���ڸ��˺�) �޵�  (�񵲺�һ��ʱ����) ����Ե���(����)
    private ISkillOwner hitSource => player.hurtSource;
    private enum DefenceChildState
    {
        Enter,
        HoldDefence,
        WaitCounterAttack,
        Exit,
    }

    private DefenceChildState defenceState;
    private DefenceChildState DefenceState
    {
        get => defenceState;
        set
        {
            defenceState = value;
            switch(defenceState)
            {
                case DefenceChildState.Enter:
                    player.PlayAnimation("EnterDefence");
                    break;
                case DefenceChildState.HoldDefence:
                    break;
                case DefenceChildState.WaitCounterAttack:
                    break;
                case DefenceChildState.Exit:
                    player.PlayAnimation("ExitDefence");
                    break;
            }
        }
    }
    public override void Enter()
    {
        player.preciseDefence = false;
        player.currentDefenceTime = 0;
        player.currentAttackTime = 0;
        // ע����˶�
        player.Model.SetRootMotionAction(OnRootMotion);
        DefenceState = DefenceChildState.Enter;
        if (player.currentEnemy != null) player.ModelTransForm.LookAt(player.currentEnemy.transform.position);
    }

    public override void Update()
    {
        player.currentDefenceTime += Time.deltaTime;
        switch (defenceState)
        {
            case DefenceChildState.Enter:
                bool enterRet = CheekAnimatorStateName("EnterDefence", out float enterAnimationTime);
                if(enterRet && enterAnimationTime >= 1)
                {
                    DefenceState = DefenceChildState.HoldDefence;
                }
                break;
            case DefenceChildState.HoldDefence:
                // �����⵽��׼���� �л���׼������״̬
                if (player.preciseDefence) DefenceState = DefenceChildState.WaitCounterAttack;

                // ����ɿ����� ���л����˳�
                if (Input.GetMouseButtonUp(1)) DefenceState = DefenceChildState.Exit;
                break;
            case DefenceChildState.WaitCounterAttack:
                player.currentAttackTime += Time.deltaTime;
                
                if (player.currentAttackTime <= player.defenceAttackTime && Input.GetMouseButtonDown(0))
                {
                    player.ChangeState(PlayerState.CounterAttack);
                }

                // ����ɿ����� ���л����˳�
                if (Input.GetMouseButtonUp(1)) DefenceState = DefenceChildState.Exit;
                break;
            case DefenceChildState.Exit:
                bool exitRet = CheekAnimatorStateName("EnterDefence", out float exitAnimationTime);
                if(exitRet && exitAnimationTime >= 1)
                {
                    player.ChangeState(PlayerState.Idle);
                }
                break;
        }
    }

    public override void Exit()
    {
        player.preciseDefence = false;
        player.Model.ClearRootMotionAction();
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}