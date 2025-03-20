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
        // ��� ��������ı�ǩ
        if (enemyTagList.Contains(other.tag))
        {
            IHurt enemy = other.GetComponentInParent<IHurt>();
            // ���ι����������λ���򲻲�������
            if (enemy != null && !enemyList.Contains(enemy))
            {
                // todo: ֪ͨ�ϼ���������
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
