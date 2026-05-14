using JetBrains.Annotations;
using UnityEngine;

public class HostileAIController : AIController 
{
    TargetingModule targetingModule;

    protected override void OnPossess(CharacterBase newCharacter)
    {
        GameManager.OnUpdateController -= Think;
        GameManager.OnUpdateController += Think;

        targetingModule = Character.GetModule<TargetingModule>();
    }
    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        GameManager.OnUpdateController -= Think;
    }

    protected override void Think(float deltaTime)
    {
        //스캔 주기마다 스캔 시도
        if(targetingModule.TryGetNewTarget(deltaTime, out GameObject newTarget))
        {
            //스캔시도 됬으면 목표에 새 타겟 넣기
            SetFocusTarget(newTarget);
        }

        //목표가 없어? 그럼 여기서 끝. 가만히 있어
        if (!FocusTarget) return;
        
        //때리던지 움직이든지 해라
        AttackOrMoveCheck();
    }

    protected void AttackOrMoveCheck()
    {
        //사거리 내 적 들어오면 공격, 안되면 이동 시도
        //일단 사거리 관련 데이터가 없어서 이동만 시도
        CommandMoveToDestination(FocusTarget.transform.position, 0.5f);
    }

    //타겟과 가까워 졌을때 호출
    public void Attack()
    {
        AttackModule atkModule = Character.GetModule<AttackModule>();
        atkModule.AttackTarget(new AttackInfo
        {
            target = FocusTarget,
            instigator = this,
            damageAmount = Character.Status.damage
        });
    }

}