using UnityEngine;

[CreateAssetMenu(menuName = "Config/SkillHitEFConfig")]
public class SkillHitEFConfig:ScriptableObject
{
    // 产生的粒子 物体
    public Skill_SpawnObj SpawnObj;
    // 命中时音效 通用
    public AudioClip AudioClip;
}