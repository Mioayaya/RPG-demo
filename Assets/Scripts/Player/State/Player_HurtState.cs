using System.Collections;
using UnityEngine;

public class Player_HurtState : PlayerStateBase
{
    private Skill_HitData hitData => player.hitData;
    private ISkillOwner hitSource => player.hurtSource;

    private float currentHardTime = 0;
    private Coroutine repelCoroutine;
    
    enum HurtChildState
    {
        // ��ͨ����
        NormalHurt,
        // ����-Down
        Down,
        // ����-Rise
        Rise
    }
    private HurtChildState hurtState;
    private HurtChildState HurtState { 
        get => hurtState; 
        set
        {
            hurtState = value;
            switch(hurtState)
            {
                case HurtChildState.NormalHurt:
                    player.PlayAnimation("Hurt");
                    break;
                case HurtChildState.Down:
                    player.PlayAnimation("Down");
                    break;
                case HurtChildState.Rise:
                    player.PlayAnimation("Rise");
                    break;
            }
        } 
    }
    public override void Enter()
    {
        currentHardTime = 0;
        if(hitData.Down) HurtState = HurtChildState.Down;
        else HurtState = HurtChildState.NormalHurt;

        // ���˾���
        if(hitData.RepelVelocity != Vector3.zero)
        {
            repelCoroutine = MonoManager.Instance.StartCoroutine(DoRepel(hitData.RepelTime,hitData.RepelVelocity));
        }
    }

    public override void Update()
    {
        // û�л��� ����
        if(repelCoroutine == null)
        {
            player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));
        }
        currentHardTime += Time.deltaTime;
        switch (HurtState)
        {
            case HurtChildState.NormalHurt:
                // Ӳֱ��� Ӳֱʱ�䵽�ˣ����һ��� ����ʱ��Ҳ����
                if (currentHardTime >= hitData.HardTime && repelCoroutine == null) player.ChangeState(PlayerState.Idle);
                break;
            case HurtChildState.Down:
                //boss.ModelTransForm.LookAt(hitSource.ModelTransForm);
                if (currentHardTime >= hitData.HardTime && repelCoroutine == null) HurtState = HurtChildState.Rise;
                break;
            case HurtChildState.Rise:
                // �������
                if(CheekAnimatorStateName("Rise",out float time) && time >= 0.99f)
                {
                    player.ChangeState(PlayerState.Idle);
                }
                break;
        }
    }

    public override void Exit()
    {
        if(repelCoroutine != null)
        {
            MonoManager.Instance.StopCoroutine(repelCoroutine);
            repelCoroutine = null;
        }
    }

    private IEnumerator DoRepel(float time, Vector3 velocity)
    {
        float currentTime = 0;

        // �����ĸΪ0
        time = time == 0 ? 0.01f : time;
        // ��λ�Ʒ����޸�Ϊ��Զ��ֵķ���
        Vector3 veloFromSource = hitSource.ModelTransForm.TransformDirection(velocity);

        while(currentTime < time)
        {
            player.CharacterController.Move(veloFromSource / time * Time.deltaTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        repelCoroutine = null;
    }
}
