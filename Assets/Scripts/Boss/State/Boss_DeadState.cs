using UnityEngine;

public class Boss_DeadState : BossStateBase
{    
    public override void Enter()
    {
        boss.PlayAnimation("Dead");

        // ���ò���Ҫ�����
        if (boss.TryGetComponent<Collider>(out var collider))
            collider.enabled = false;

        // ֹͣ���п����������е�Э��
        boss.StopAllCoroutines();
    }

    public override void Update()
    {
        if(boss == null) Debug.Log(1);

        if (boss == null)
        {
            return;
        }

        if(CheekAnimatorStateName("Dead", out float animationTime) && animationTime >= 0.99f)
        {
            //��������
            boss.gameObject.SetActive(false);
            boss.CharacterDestory();
            
        }        
    }

}
