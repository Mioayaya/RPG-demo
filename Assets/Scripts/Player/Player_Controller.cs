using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player_Controller : CharacterBase
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    #region ��������Ϣ

    [Header("����")]
    public float preciseDefenceTime = 0.5f; // ��׼����ʱ��
    public float defenceAttackTime = 1f;    // ����ʱ��
    [SerializeField] public Skill_SpawnObj defenceSpawnObj;     // ��׼����Ч
    [SerializeField] public SkillConfig counterAttackConfig;    // ��������
    [NonSerialized] public bool preciseDefence = false;         // �Ƿ�׼����
    [NonSerialized] public float currentDefenceTime = 0;        // �񵲼�ʱ
    [NonSerialized] public float currentAttackTime = 0;         // ������Чʱ��

    [Header("��������")]
    public float rotateSpeed = 5;
    public float walk2RunTransition = 1;
    public float walkSpeed = 1;
    public float runSpeed = 1;
    public float jumpPower = 1.5f;
    public float moveSpeedForJump = 7;
    public float moveSpeedForAirDown = 3;
    public float idleStayTime = 1f;

    [Header("�������")]
    public List<SkillInfo> skillInfoList = new List<SkillInfo>();

    [Header("ǿ���չ�")]
    public StandAttackEffectType standEffectType = StandAttackEffectType.Normal;
    public SkillConfig[] enhancedStandAttackConfigs;
    [NonSerialized]public SkillConfig[] currentStandAttackConfigs;  // ��ǰ�չ�����״̬
    
    [Header("�������")]
    public Transform detectPoint;  // ������ĵ�
    public float detectRadius = 10f; // ���а뾶
    public LayerMask enemyLayer;    // ���˲㼶
    [NonSerialized] public Boss_Controller currentEnemy;  // ��ǰ���е���
    #endregion

    public enum StandAttackEffectType
    {
        Normal,Enhanced
    }

    public enum BeforeState
    {
        Idle,Walk,Run
    }
    
    // Ĭ���Ǵ���״̬
    [NonSerialized]public BeforeState beforeState = BeforeState.Idle;

    private void Start()
    {
        // �������
        Cursor.lockState = CursorLockMode.Locked;
        Init();
        ChangeStandAttackType();
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        UpDateSkillCDTime();
        // ����
        if (Input.GetMouseButtonDown(2))
        {
            // �״ΰ��� Ѱ������ĵ���
            if (currentEnemy == null)
            {
                FindNearestEnemy();                
            }
            // ȡ������
            else ClearDetectEnemy();
        }
    }
#region ����
    private void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(
            detectPoint.position,
            detectRadius,
            enemyLayer
        );        

        if (enemies.Length == 0) return;

        // ʹ��LINQ�����ҵ�������ˣ���ƽ�������Ż����ܣ�
        var nearest = enemies
            .Where(c => c.GetComponent<Boss_Controller>() != null)
            .OrderBy(c => (c.transform.position - detectPoint.position).sqrMagnitude)
            .FirstOrDefault();        

        if (nearest != null)
        {
            currentEnemy = nearest.GetComponent<Boss_Controller>();
            //currentEnemy.test;
        }

        if(currentEnemy != null)
        {
            currentEnemy.ToggleIconAlpha();
        }
    }

    private void ClearDetectEnemy()
    {
        if (currentEnemy != null)
        {
            currentEnemy.ToggleIconAlpha();
            currentEnemy = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(detectPoint.position, detectRadius);
        }
    }

#endregion

    // ��ѡ����������Ŀ�꣨���Ŀ���ƶ��󳬳���Χ���Զ�ȡ����
    void FixedUpdate()
    {
        if (currentEnemy != null &&
            Vector3.Distance(detectPoint.position, currentEnemy.transform.position) > detectRadius
            )
        {
            ClearDetectEnemy();
        }
    }

    // �����ÿ��ӻ���Χ
    

    public void ChangeState(PlayerState playerState, bool reCurrentState = false)
    {
        switch(playerState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<Player_IdleState>(reCurrentState);
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<Player_MoveState>(reCurrentState);
                break;
            case PlayerState.Jump:
                stateMachine.ChangeState<Player_JumpState>(reCurrentState);
                break;
            case PlayerState.AirDown:
                stateMachine.ChangeState<Player_AirDownState>(reCurrentState);
                break;
            case PlayerState.Roll:
                stateMachine.ChangeState<Player_RollState>(reCurrentState);
                break;
            case PlayerState.RunSide:
                stateMachine.ChangeState<Player_RunSideState>(reCurrentState);
                break;
            case PlayerState.StandAttack:
                stateMachine.ChangeState<Player_StandAttack>(reCurrentState);
                break;
            case PlayerState.Hurt:
                stateMachine.ChangeState<Player_HurtState>(reCurrentState);
                break;
            case PlayerState.Defence:
                stateMachine.ChangeState<Player_DefenceState>(reCurrentState);
                break;
            case PlayerState.CounterAttack:
                stateMachine.ChangeState<Player_CounterAttackState>(reCurrentState);
                break;
            case PlayerState.SkillAttack:
                stateMachine.ChangeState<Player_SkillAttack>(reCurrentState);
                break;

        }
    }

    public override void OnHit(IHurt target, Vector3 hitPosition)
    {
        Debug.Log(currentHitIndex);
        if (currentHitIndex >= currentSkillConfig.AttackData.Length) return;
        // �õ���һ�εĹ�������
        Skill_AttackData attackData = currentSkillConfig.AttackData[currentHitIndex];
        // ���ɻ����������õ�Ч��
        StartCoroutine(DoSkillHitEffect(attackData.SkillHitEFConfig, hitPosition));
        // ����Ч����
        if (attackData.ScreenImpulseValue != 0) ScreenImpulse(attackData.ScreenImpulseValue);
        StartFreezeFrame(attackData.FreezeFrameTime);
        //StartFreezeGameTime(attackData.FreezeGameTime);
        // todo: �����˺�����
        target.Hurt(attackData.HitData, this);
    }



    public void ScreenImpulse(float force)
    {
        impulseSource.GenerateImpulse(force);
    }

    public override void Hurt(Skill_HitData hitData, ISkillOwner hurtSource)
    {
        // �����ǰ�Ǹ�״̬�Ļ�
        if(stateMachine.CurrentStateType == typeof(Player_DefenceState))
        {
            // ��ʱ��С�ڵ��ھ�׼��ʱ��Ļ� ������׼�񵲣�������Ч����Ч ���Ҳ�����
            if(currentDefenceTime <= preciseDefenceTime)
            {
                this.hurtSource = hurtSource;
                preciseDefence = true;
                SpawnSkillObject(defenceSpawnObj);
                return;
            }
            // ���� ������뵽����״̬��
            // todo: �����յ����˺�����
            base.Hurt(hitData, hurtSource);
            return;
        }
        // todo: Boss ����׶�
        base.Hurt(hitData, hurtSource);
        
        // �л�������״̬ 
        // todo: ����boss���⹥���½�������״̬
        // ChangeState(PlayerState.Hurt, true);
    }

    /// <summary>
    /// ��鲢�ҽ��뼼��״̬
    /// </summary>
    /// <returns></returns>
    
    private Coroutine enhancedAttackCoroutine;
    public bool CheckAndEnterSkillState()
    {

        if (!CanSwitchSkill) return false;

        // ������м�����û��CD��������Ұ��˶�Ӧ�ļ�λ        
        for (int i = 0; i < skillInfoList.Count; i++)
        {
            if (skillInfoList[i].currentTime == 0 && Input.GetKeyDown(skillInfoList[i].keyCode))
            {
                // �����ǰ�� ����ǿ���չ�״̬
                if (skillInfoList[i].isEnhanced)
                {
                    if (enhancedAttackCoroutine != null) StopCoroutine(enhancedAttackCoroutine);
                    enhancedAttackCoroutine = StartCoroutine(EnhancedAttackDuration(skillInfoList[i].stillTime));

                    standEffectType = StandAttackEffectType.Enhanced;
                    ChangeStandAttackType();
                    skillInfoList[i].currentTime = skillInfoList[i].cdTime;
                    return false;
                }
                
                // �ͷż���                
                ChangeState(PlayerState.SkillAttack, true);
                Player_SkillAttack skillAttack = (Player_SkillAttack)stateMachine.currentState;
                skillAttack.InitData(skillInfoList[i].skillConfig);
                
                // �ü���cd

                skillInfoList[i].currentTime = skillInfoList[i].cdTime;

                return true;
            }
        }
        return false;
    }

    private void UpDateSkillCDTime()
    {
        for (int i = 0; i < skillInfoList.Count; i++)
        {

            skillInfoList[i].currentTime = Mathf.Clamp(skillInfoList[i].currentTime - Time.deltaTime, 0, skillInfoList[i].cdTime);
            skillInfoList[i].cdMaskImage.fillAmount = skillInfoList[i].currentTime / skillInfoList[i].cdTime;
        }
    }

    private void ChangeStandAttackType()
    {
        if (standEffectType == StandAttackEffectType.Normal) currentStandAttackConfigs = standAttackConfigs;
        else currentStandAttackConfigs = enhancedStandAttackConfigs;
    }

    // ����Э�̴���ǿ������ʱ��
    private IEnumerator EnhancedAttackDuration(float time)
    {
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // ����ʱ������ָ���ͨ״̬
        standEffectType = StandAttackEffectType.Normal;
        ChangeStandAttackType();
        enhancedAttackCoroutine = null;
    }

}
