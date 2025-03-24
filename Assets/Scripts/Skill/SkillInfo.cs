using System;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class SkillInfo
{
    public enum SkillType
    {
        None,
        Enhanced,
        ChangeWeapon
    }

    public KeyCode keyCode;
    public SkillType skillType;    
    public SkillConfig skillConfig;
    public float stillTime; // 持续时间
    public float cdTime;
    [NonSerialized]public float currentTime;
    public Image cdMaskImage;
}