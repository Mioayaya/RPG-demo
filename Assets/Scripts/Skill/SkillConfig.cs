using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Config/Skill")]
public class SkillConfig : ScriptableObject
{
    // ���ܶ�������
    public string AnimationName;
    // �ͷż������� 
    public Skill_ReleaseData ReleaseData;
    // ��������
    public Skill_AttackData[] AttackData;
}

/// <summary>
/// �����ͷ�����
/// </summary>
[Serializable]
public class Skill_ReleaseData
{
    // ��������
    public Skill_SpawnObj Release_SpawnObj;
    // ��Ч
    public AudioClip Release_AudioClip;
}

/// <summary>
/// ���ܲ�������
/// </summary>
[Serializable]
public class Skill_SpawnObj
{
    // ���ɵ�Ԥ����
    public GameObject Prefab;
    // ���ɵ���Ч
    public AudioClip AudioClip;
    // λ��
    public Vector3 Position;
    // ��ת
    public Vector3 Rotation;
    // ����
    public Vector3 Scale = Vector3.one;
    // �ӳ�ʱ��
    public float Time;
}

/// <summary>
/// ���ܹ�������
/// �ɶ����¼�����
/// </summary>
[Serializable]
public class Skill_AttackData
{
    // �����ͷ�����
    public Skill_SpawnObj Attack_SpawnObj;

    // û�й�����Ч�������й�����Ч
    public AudioClip AudioClipWithoutSpawnObj;

    /* �������� */
    public Skill_HitData HitData;
    // ��Ļ��
    public float ScreenImpulseValue;
    // ����Ч��
    public float FreezeFrameTime;
    // ʱ��ֹͣ
    public float FreezeGameTime;
    // ����Ч��
    public SkillHitEFConfig SkillHitEFConfig;

}

/// <summary>
/// ��������
/// </summary>
[Serializable]
public class Skill_HitData
{
    // �˺���ֵ 
    public float DamgeValue;
    // ����Ӳֱʱ��
    public float HardTime;
    // ���ɡ����˳̶�
    public Vector3 RepelVelocity;
    // ���ɡ����˵Ĺ���ʱ��
    public float RepelTime;
    // �Ƿ���Ҫ����
    public bool Down;
}