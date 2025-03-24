using UnityEngine;

public class Boss_DeadState : BossStateBase
{    
    public override void Enter()
    {
        boss.PlayAnimation("Dead");

        // 禁用不必要的组件
        if (boss.TryGetComponent<Collider>(out var collider))
            collider.enabled = false;

        // 停止所有可能正在运行的协程
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
            //销毁物体
            boss.gameObject.SetActive(false);
            boss.CharacterDestory();
            
        }        
    }

}
