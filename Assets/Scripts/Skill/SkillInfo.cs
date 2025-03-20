using System;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class SkillInfo
{
    public KeyCode keyCode;
    public bool isEnhanced;  // 是否是强化技能
    public SkillConfig skillConfig;
    public float stillTime; // 持续时间
    public float cdTime;
    [NonSerialized]public float currentTime;
    public Image cdMaskImage;
}