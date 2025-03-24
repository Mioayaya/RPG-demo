using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterBase : MonoBehaviour, IStateMachinerOwner, ISkillOwner, IHurt
{
    public Transform ModelTransForm => Model.transform;
    public StateMachine stateMachine;

    [SerializeField] protected ModelBase model;
    public ModelBase Model { get => model; }

    [SerializeField] protected CharacterController characterController;
    public CharacterController CharacterController { get => characterController; }

    [SerializeField] protected AudioSource audioSource;
    public Image cdMaskImage;



    #region 配置信息
    public AudioClip[] footStepAudioClips;
    public List<string> enemyTagList;
    public SkillConfig[] standAttackConfigs;
    public float gravity = -9.8f;
    public float Hp = 100f;
    public float maxHp = 100f;
    #endregion

    public virtual void Init()
    {
        Model.Init(this, enemyTagList);
        stateMachine = new StateMachine();
        stateMachine.Init(this);
        canSwitchSkill = true;
    }

    #region 技能相关

    protected SkillConfig currentSkillConfig;
    protected int currentHitIndex = 0;
    protected bool canSwitchSkill;
    public bool CanSwitchSkill { get => canSwitchSkill; }
    public Skill_HitData hitData { get; protected set; }
    public ISkillOwner hurtSource { get; protected set; }

    public void StartAttack(SkillConfig skillConfig)
    {
        canSwitchSkill = false; // 防止立刻切换技能
        currentSkillConfig = skillConfig;
        currentHitIndex = 0;
        // 播放技能动画
        PlayAnimation(currentSkillConfig.AnimationName);
        // 技能释放物体
        SpawnSkillObject(currentSkillConfig.ReleaseData.Release_SpawnObj);
        // 技能释放音效 <角色语音等>
        PlayAudio(currentSkillConfig.ReleaseData.Release_AudioClip);

    }

    // 技能释放动画
    public void ReleaseSkill()
    {

    }

    // 技能攻击动画
    public void StartNewSkillHit()
    {
        SpawnSkillObject(currentSkillConfig.AttackData[currentHitIndex].Attack_SpawnObj);
        // 没有obj但是有音效
        PlayAudio(currentSkillConfig.AttackData[currentHitIndex].AudioClipWithoutSpawnObj);
    }

    // 技能攻击动画 (使用近战武器攻击)
    public void StartSkillHit(int weaponIndex)
    {        
        SpawnSkillObject(currentSkillConfig.AttackData[currentHitIndex].Attack_SpawnObj);
        // 没有obj但是有音效
        PlayAudio(currentSkillConfig.AttackData[currentHitIndex].AudioClipWithoutSpawnObj);
    }

    public void StopSkillHit(int weaponIndex)
    {
        currentHitIndex += 1;
    }

    public void SkillCanSwitch()
    {
        canSwitchSkill = true;
    }

    protected void SpawnSkillObject(Skill_SpawnObj spawnObj)
    {
        if (spawnObj != null && spawnObj.Prefab != null) StartCoroutine(DoSpawnObject(spawnObj));        
    }

    protected IEnumerator DoSpawnObject(Skill_SpawnObj spawnObj)
    {
        Transform currentPoint = Model.transform;
        // 延迟时间
        yield return new WaitForSeconds(spawnObj.Time);
        // 生成特效
        //GameObject skillObj = GameObject.Instantiate(spawnObj.Prefab, spawnPos, spawnRot);
        GameObject skillObj = GameObject.Instantiate(spawnObj.Prefab, null);
        // 一般特效的生成是相对于主角的
        //Vector3 spawnPos = currentPoint.TransformPoint(spawnObj.Position);
        //Quaternion spawnRot = currentPoint.rotation * Quaternion.Euler(spawnObj.Rotation);

        // 始终以角色面向为基准点 
        skillObj.transform.position = currentPoint.TransformPoint(spawnObj.Position);
        skillObj.transform.eulerAngles = currentPoint.eulerAngles + spawnObj.Rotation;
        skillObj.transform.localScale = spawnObj.Scale;
        PlayAudio(spawnObj.AudioClip);

        if(skillObj.TryGetComponent<SkillObjBase>(out SkillObjBase skillobject))
        {
            skillobject.Init(enemyTagList, OnHit);
        }
    }

    public virtual void OnHit(IHurt target, Vector3 hitPosition)
    {
        // 拿到这一段的攻击数据
        Skill_AttackData attackData = currentSkillConfig.AttackData[currentHitIndex];
        // 生成基于命中配置的效果
        StartCoroutine(DoSkillHitEffect(attackData.SkillHitEFConfig, hitPosition));
        StartFreezeFrame(attackData.FreezeFrameTime);
        //StartFreezeGameTime(attackData.FreezeGameTime);
        // todo: 传递伤害数据
        target.Hurt(attackData.HitData, this);
    }

    protected void StartFreezeFrame(float time)
    {
        if (time > 0) StartCoroutine(DoFreezeFrame(time));
    }

    protected IEnumerator DoFreezeFrame(float time)
    {
        Model.Animator.speed = 0.01f;
        yield return new WaitForSeconds(time);
        Model.Animator.speed = 1;
    }

    protected void StartFreezeGameTime(float time)
    {
        if (time > 0) StartCoroutine(DoFreezeGameTime(time));
    }

    protected IEnumerator DoFreezeGameTime(float time)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }

    protected IEnumerator DoSkillHitEffect(SkillHitEFConfig hitEFConfig, Vector3 spawnPoint)
    {
        if (hitEFConfig == null) yield break;
        if (hitEFConfig.SpawnObj != null && hitEFConfig.SpawnObj.Prefab != null)
        {
            yield return new WaitForSeconds(hitEFConfig.SpawnObj.Time);
            GameObject temp = Instantiate(hitEFConfig.SpawnObj.Prefab);
            temp.transform.position = spawnPoint + hitEFConfig.SpawnObj.Position;
            temp.transform.LookAt(Model.transform.position);
            temp.transform.eulerAngles += hitEFConfig.SpawnObj.Rotation;

            // 让特效始终垂直于角色
            //Quaternion effectRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Model.transform.forward, Vector3.up));
            //temp.transform.rotation = effectRotation * Quaternion.Euler(hitEFConfig.SpawnObj.Rotation);

            PlayAudio(hitEFConfig.SpawnObj.AudioClip);
        }
        PlayAudio(hitEFConfig.AudioClip);
    }

    public void OnSkillOver()
    {
        canSwitchSkill = true;
    }

    #endregion

    public void PlayAnimation(string animationName, float fixedTransitionDuration = 0.25f)
    {
        if (animationName == null) return;
        model.Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }

    public void OnFootStep()
    {
        audioSource.PlayOneShot(footStepAudioClips[UnityEngine.Random.Range(0, footStepAudioClips.Length)]);
    }

    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null) audioSource.PlayOneShot(audioClip);
    }

    public virtual void Hurt(Skill_HitData hitData, ISkillOwner hurtSource)
    {
        this.hitData = hitData;
        this.hurtSource = hurtSource;
        Hp -= hitData.DamgeValue;
        cdMaskImage.fillAmount = Hp / maxHp;
    }

    public virtual void CharacterDestory()
    {
        gameObject.SetActive(false);
        stateMachine.Stop();
        Destroy(gameObject);
    }
    
}