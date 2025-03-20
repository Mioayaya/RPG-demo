using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ISkillOwner
{
    Transform ModelTransForm { get; }

    /// <summary>
    /// ISkillOwner <动画> 技能释放动作
    /// </summary>
    void ReleaseSkill();

    /// <summary>
    /// ISkillOwner <动画> 开始攻击
    /// </summary>
    /// <param name="weaponIndex">武器序号</param>
    void StartSkillHit(int weaponIndex);

    /// <summary>
    /// ISkillOwner <动画> 特效判定开始、特效释放
    /// </summary>
    void StartNewSkillHit();

    /// <summary>
    /// ISkillOwner <动画> 停止攻击
    /// </summary>
    /// <param name="weaponIndex">武器序号</param>
    void StopSkillHit(int weaponIndex);

    /// <summary>
    /// ISkillOwner <动画> 可以取消后摇
    /// </summary>
    void SkillCanSwitch();

    /// <summary>
    /// ISkillOwner 收到攻击
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hitPosition"></param>
    void OnHit(IHurt target,Vector3 hitPosition);

    /// <summary>
    /// ISkillOwner <动画> 脚步声
    /// </summary>
    void OnFootStep();
}