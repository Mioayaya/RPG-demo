using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Config/Skill")]
public class SkillConfig : ScriptableObject
{
    // 技能动画名称
    public string AnimationName;
    // 释放技能数据 
    public Skill_ReleaseData ReleaseData;
    // 攻击数据
    public Skill_AttackData[] AttackData;
}

/// <summary>
/// 技能释放数据
/// </summary>
[Serializable]
public class Skill_ReleaseData
{
    // 播放粒子
    public Skill_SpawnObj Release_SpawnObj;
    // 音效
    public AudioClip Release_AudioClip;
}

/// <summary>
/// 技能产生物体
/// </summary>
[Serializable]
public class Skill_SpawnObj
{
    // 生成的预制体
    public GameObject Prefab;
    // 生成的音效
    public AudioClip AudioClip;
    // 位置
    public Vector3 Position;
    // 旋转
    public Vector3 Rotation;
    // 缩放
    public Vector3 Scale = Vector3.one;
    // 延迟时间
    public float Time;
}

/// <summary>
/// 技能攻击数据
/// 由动画事件控制
/// </summary>
[Serializable]
public class Skill_AttackData
{
    // 攻击释放数据
    public Skill_SpawnObj Attack_SpawnObj;

    // 没有攻击特效，但是有攻击音效
    public AudioClip AudioClipWithoutSpawnObj;

    /* 命中数据 */
    public Skill_HitData HitData;
    // 屏幕震动
    public float ScreenImpulseValue;
    // 卡肉效果
    public float FreezeFrameTime;
    // 时间停止
    public float FreezeGameTime;
    // 命中效果
    public SkillHitEFConfig SkillHitEFConfig;

}

/// <summary>
/// 命中数据
/// </summary>
[Serializable]
public class Skill_HitData
{
    // 伤害数值 
    public float DamgeValue;
    // 敌人硬直时间
    public float HardTime;
    // 击飞、击退程度
    public Vector3 RepelVelocity;
    // 击飞、击退的过度时间
    public float RepelTime;
    // 是否需要击倒
    public bool Down;
}