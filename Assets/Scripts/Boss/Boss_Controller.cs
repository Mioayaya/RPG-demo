using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss_Controller : CharacterBase
{
    public Player_Controller targetPlayer;
    public Transform attackTarget;
    public NavMeshAgent navMeshAgent;
    public float walkRang = 5;
    public float walkSpeed;
    public float runSpeed;
    public float standAttackRang;
    public float standAttackCDTime = 3f;

    public Image[] TrackIconList;
    private void Start()
    {
        Init();
        ChangeState(BossState.Idle);
        foreach(var item in TrackIconList) item.color = new Color(1, 1, 1, 0);        
    }

    // �л�͸����
    public void ToggleIconAlpha()
    {
        foreach (var item in TrackIconList)
        {
            float newAlpha = item.color.a == 0 ? 1f : 0f;            
            item.color = new Color(1, 1, 1, newAlpha);
        }
    }

    public void ChangeState(BossState bossState,bool reCurrentState = false)
    {
        switch(bossState)
        {
            case BossState.Idle:
                stateMachine.ChangeState<Boss_IdleState>(reCurrentState);
                break;
            case BossState.Walk:
                stateMachine.ChangeState<Boss_WalkState>(reCurrentState);
                break;
            case BossState.Run:
                stateMachine.ChangeState<Boss_RunState>(reCurrentState);
                break;
            case BossState.Hurt:
                stateMachine.ChangeState<Boss_HurtState>(reCurrentState);
                break;
            case BossState.Attack:
                stateMachine.ChangeState<Boss_AttackState>(reCurrentState);
                break;

        }
    }


    public override void Hurt(Skill_HitData hitData,ISkillOwner hurtSource)
    {
        // todo: Boss ����׶�
        base.Hurt(hitData, hurtSource);
        ChangeState(BossState.Hurt, true);
    }


    #region UnityEditor
#if UNITY_EDITOR

    [ContextMenu("SetHurtCollider")]
    private void SetHurtCollider()
    {
        // �������е���ײ��ΪHurtCollider
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for(int i=0; i< colliders.Length; i++)
        {
            // �ų�����
            if (colliders[i].GetComponent<Weapon_Controller>() == null)
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("HurtCollider");
                colliders[i].gameObject.tag = "Enemy";
            }
        }
        // ��ǳ����޸�
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }


#endif
    #endregion

}
