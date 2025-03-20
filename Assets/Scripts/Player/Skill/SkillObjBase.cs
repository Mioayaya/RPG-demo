using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class SkillObjBase : MonoBehaviour
{
    [SerializeField] private new BoxCollider collider;
    private List<string> enemyTagList;
    private List<IHurt> enemyList = new List<IHurt>();
    private Action<IHurt, Vector3> onHitAction;
    private float hitCD = 0.5f;
    private bool isCoolingDown;


    public virtual void Init(List<string> enemyTagList, Action<IHurt, Vector3> onHitAction)
    {
        collider.enabled = true;
        this.enemyTagList = enemyTagList;
        this.onHitAction = onHitAction;
    }


    protected virtual void OnTriggerStay(Collider other)
    {
        if (isCoolingDown) return;
        StartCoroutine(ResetCoolDown());
        
        if (enemyTagList == null) return;
        // 检测 攻击对象的标签
        if (enemyTagList.Contains(other.tag))
        {
            IHurt enemy = other.GetComponentInParent<IHurt>();
            // 本次攻击过这个单位，则不产生攻击
            if (enemy != null && !enemyList.Contains(enemy))
            {
                // todo: 通知上级处理命中
                onHitAction?.Invoke(enemy, other.ClosestPoint(transform.position));
                enemyList.Add(enemy);
            }
        }
    }

    IEnumerator ResetCoolDown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(hitCD);
        isCoolingDown = false;
    }
}
