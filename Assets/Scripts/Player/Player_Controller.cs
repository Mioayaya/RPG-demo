using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player_Controller : CharacterBase
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    #region 配置类信息

    [Header("弹反")]
    public float preciseDefenceTime = 0.5f; // 精准弹反时间
    public float defenceAttackTime = 1f;    // 反击时间
    [SerializeField] public Skill_SpawnObj defenceSpawnObj;     // 精准格挡特效
    [SerializeField] public SkillConfig counterAttackConfig;    // 弹反配置
    [NonSerialized] public bool preciseDefence = false;         // 是否精准弹反
    [NonSerialized] public float currentDefenceTime = 0;        // 格挡计时
    [NonSerialized] public float currentAttackTime = 0;         // 弹反有效时间

    [Header("基础配置")]
    public float rotateSpeed = 5;
    public float walk2RunTransition = 1;
    public float walkSpeed = 1;
    public float runSpeed = 1;
    public float jumpPower = 1.5f;
    public float moveSpeedForJump = 7;
    public float moveSpeedForAirDown = 3;
    public float idleStayTime = 1f;

    [Header("技能相关")]
    public List<SkillInfo> skillInfoList = new List<SkillInfo>();

    [Header("强化普攻")]
    public StandAttackEffectType standEffectType = StandAttackEffectType.Normal;
    public SkillConfig[] enhancedStandAttackConfigs;
    [NonSerialized]public SkillConfig[] currentStandAttackConfigs;  // 当前普攻配置状态
    
    [Header("索敌相关")]
    public Transform detectPoint;  // 检测中心点
    public float detectRadius = 10f; // 索敌半径
    public LayerMask enemyLayer;    // 敌人层级
    [NonSerialized] public Boss_Controller currentEnemy;  // 当前索敌敌人
    #endregion

    public enum StandAttackEffectType
    {
        Normal,Enhanced
    }

    public enum BeforeState
    {
        Idle,Walk,Run
    }
    
    // 默认是待机状态
    [NonSerialized]public BeforeState beforeState = BeforeState.Idle;

    private void Start()
    {
        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Init();
        ChangeStandAttackType();
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        UpDateSkillCDTime();
        // 索敌
        if (Input.GetMouseButtonDown(2))
        {
            // 首次按下 寻找最近的敌人
            if (currentEnemy == null)
            {
                FindNearestEnemy();                
            }
            // 取消锁定
            else ClearDetectEnemy();
        }
    }
#region 索敌
    private void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(
            detectPoint.position,
            detectRadius,
            enemyLayer
        );        

        if (enemies.Length == 0) return;

        // 使用LINQ快速找到最近敌人（按平方距离优化性能）
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

    // 可选：持续跟踪目标（如果目标移动后超出范围则自动取消）
    void FixedUpdate()
    {
        if (currentEnemy != null &&
            Vector3.Distance(detectPoint.position, currentEnemy.transform.position) > detectRadius
            )
        {
            ClearDetectEnemy();
        }
    }

    // 调试用可视化范围
    

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
        // 拿到这一段的攻击数据
        Skill_AttackData attackData = currentSkillConfig.AttackData[currentHitIndex];
        // 生成基于命中配置的效果
        StartCoroutine(DoSkillHitEffect(attackData.SkillHitEFConfig, hitPosition));
        // 播放效果类
        if (attackData.ScreenImpulseValue != 0) ScreenImpulse(attackData.ScreenImpulseValue);
        StartFreezeFrame(attackData.FreezeFrameTime);
        //StartFreezeGameTime(attackData.FreezeGameTime);
        // todo: 传递伤害数据
        target.Hurt(attackData.HitData, this);
    }



    public void ScreenImpulse(float force)
    {
        impulseSource.GenerateImpulse(force);
    }

    public override void Hurt(Skill_HitData hitData, ISkillOwner hurtSource)
    {
        // 如果当前是格挡状态的话
        if(stateMachine.CurrentStateType == typeof(Player_DefenceState))
        {
            // 格挡时间小于等于精准格挡时间的话 触发精准格挡，生成音效和特效 并且不受伤
            if(currentDefenceTime <= preciseDefenceTime)
            {
                this.hurtSource = hurtSource;
                preciseDefence = true;
                SpawnSkillObject(defenceSpawnObj);
                return;
            }
            // 否则 不会进入到受伤状态，
            // todo: 并且收到的伤害降低
            base.Hurt(hitData, hurtSource);
            return;
        }
        // todo: Boss 霸体阶段
        base.Hurt(hitData, hurtSource);
        
        // 切换到受伤状态 
        // todo: 仅在boss特殊攻击下进入受伤状态
        // ChangeState(PlayerState.Hurt, true);
    }

    /// <summary>
    /// 检查并且进入技能状态
    /// </summary>
    /// <returns></returns>
    
    private Coroutine enhancedAttackCoroutine;
    public bool CheckAndEnterSkillState()
    {

        if (!CanSwitchSkill) return false;

        // 检测所有技能有没有CD，并且玩家按了对应的键位        
        for (int i = 0; i < skillInfoList.Count; i++)
        {
            if (skillInfoList[i].currentTime == 0 && Input.GetKeyDown(skillInfoList[i].keyCode))
            {
                // 如果当前是 进入强化普攻状态
                if (skillInfoList[i].isEnhanced)
                {
                    if (enhancedAttackCoroutine != null) StopCoroutine(enhancedAttackCoroutine);
                    enhancedAttackCoroutine = StartCoroutine(EnhancedAttackDuration(skillInfoList[i].stillTime));

                    standEffectType = StandAttackEffectType.Enhanced;
                    ChangeStandAttackType();
                    skillInfoList[i].currentTime = skillInfoList[i].cdTime;
                    return false;
                }
                
                // 释放技能                
                ChangeState(PlayerState.SkillAttack, true);
                Player_SkillAttack skillAttack = (Player_SkillAttack)stateMachine.currentState;
                skillAttack.InitData(skillInfoList[i].skillConfig);
                
                // 让技能cd

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

    // 新增协程处理强化持续时间
    private IEnumerator EnhancedAttackDuration(float time)
    {
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 持续时间结束恢复普通状态
        standEffectType = StandAttackEffectType.Normal;
        ChangeStandAttackType();
        enhancedAttackCoroutine = null;
    }

}
