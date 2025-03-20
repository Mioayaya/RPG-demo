using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Controller : MonoBehaviour
{
    [SerializeField]private new Collider collider;

    private List<string> enemyTagList;
    private List<IHurt> enemyList;
    private Action<IHurt, Vector3> onHitAction;

    public void Init(List<string> enemyTagList,Action<IHurt,Vector3> onHitAction)
    {
        collider.enabled = false;
        this.enemyTagList = enemyTagList;
        this.onHitAction = onHitAction;
        enemyList = new List<IHurt>();
        //collider.isTrigger = false;
    }

    public void StartSkillHit()
    {
        collider.enabled = true;
        //collider.isTrigger = true;
    }

    public void StopSkillHit()
    {
        collider.enabled = false;
        //collider.isTrigger = false;
        enemyList.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemyTagList == null) return;
        // ��� ��������ı�ǩ
        if(enemyTagList.Contains(other.tag))
        {
            IHurt enemy = other.GetComponentInParent<IHurt>();
            // ���ι����������λ���򲻲�������
            if(enemy != null && !enemyList.Contains(enemy))
            {
                // todo: ֪ͨ�ϼ���������
                onHitAction?.Invoke(enemy, other.ClosestPoint(transform.position));
                enemyList.Add(enemy);
            }
        }
    }
}
