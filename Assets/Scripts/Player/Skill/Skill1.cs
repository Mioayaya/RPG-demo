using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill1 : SkillObjBase
{
    public override void Init(List<string> enemyTagList, Action<IHurt, Vector3> onHitAction)
    {
        base.Init(enemyTagList, onHitAction);
        Destroy(gameObject,2f);
        //Invoke(nameof(StartSkillHit), 0.5f);
    }
}
