using UnityEngine;

public class HostileAIController : AIController 
{
    TargetingModule targetingModule;
    AttackModule atkModule;
    HitPointModule targetHPModule;
    float targetRadius;

    protected override void OnPossess(CharacterBase newCharacter)
    {
        GameManager.OnUpdateController -= Think;
        GameManager.OnUpdateController += Think;

        newCharacter.OnFaint -= OnFaint;
        newCharacter.OnFaint += OnFaint;

        targetingModule = Character.GetModule<TargetingModule>();
        atkModule = Character.GetModule<AttackModule>();
    }
    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        GameManager.OnUpdateController -= Think;
        oldCharacter.OnFaint -= OnFaint;
    }

    protected override void OnFocusTargetChanged(GameObject oldTarget, GameObject newTarget)
    {
        base.OnFocusTargetChanged(oldTarget, newTarget);
        if (newTarget != null)
        {
            //매 프레임마다 타겟의 radius를 가져오면 일이 너무 많을 것 같아서, 타겟이 바뀔 때 이를 저장한다
            targetRadius = newTarget.GetComponent<CharacterBase>().Status.colliderRadius;
            targetHPModule = newTarget.GetComponent<HitPointModule>();
        }
        else
        {
            //타겟이 null이니 hpmodule도 초기화
            targetHPModule = null;
        }
    }

    protected override void Think(float deltaTime)
    {
        //나 죽었으면 생각을 중지
        if (!Character || !Character.IsAlive) return;

        //target이 죽었는지 살았는지 체크
        if (!IsTargetAlive())
        {
            //우선 비워주기
            SetFocusTarget(null);
            targetingModule.ForceScanReady();
        }

        //스캔 주기마다 스캔 시도
        if (targetingModule.TryGetNewTarget(deltaTime, out GameObject newTarget))
        {
            //스캔시도 됬으면 기존 목표와 같은지 체크, 다르면 그때 넣기
            if(newTarget != FocusTarget)
            {
                SetFocusTarget(newTarget);
            }
        }

        //목표가 없어? 그럼 여기서 끝. 가만히 있어
        if (!FocusTarget) 
        {
            CommandStop();
            return;
        }
        
        //때리던지 움직이든지 해라
        AttackOrMove();
    }

    protected bool IsTargetAlive()
    {
        if (!FocusTarget || !targetHPModule || targetHPModule.IsEmpty)
        {
            return false;
        }
        return true;
    }

    protected void AttackOrMove()
    {
        //안전장치
        if (!FocusTarget)
        {
            CommandStop();
            return;
        }

        //공격 가능하냐?
        //나와 타겟의 거리 차이. 그 중 캡슐 콜라이더의 radius를 빼면 타겟과 나의 실제 거리 차이가 나온다
        //이때, 완전히 딱 붙는것을 방지하기 위해 아주 약갼의 여유공간을 두고 그 안에 들어오면 공격 가능하다

        //사거리 내 적 들어오면 공격, 안되면 이동 시도

        //실제 거리 계산
        float targetDistance = Vector3.Distance(transform.position, FocusTarget.transform.position);
        //나와 상대의 radius만큼을 빼기
        float attackRange = targetDistance - (Character.Status.colliderRadius + targetRadius);

        //만약 여유공간 만큼 들어왔다면 공격, 아니면 이동
        if (attackRange < 0.075f)
        {
            //여유공간 만큼 들어왔다면 자동으로 이동은 안하게된다
            TryAttack();
        }
        else
        {
            CommandMoveToDestination(FocusTarget.transform.position, 0.5f);
        }
    }

    //타겟과 가까워 졌을때 호출
    public void TryAttack()
    {
        //공격 쿨타임이 아니면 공격하기
        if (!atkModule.IsAttackCooldown)
        {
            atkModule.AttackTarget(new AttackInfo
            {
                target = FocusTarget,
                instigator = this,
                damageAmount = Character.Status.damage
            });
        }
    }

    //죽었을 때, 내 모든 활동을 정지해야한다
    public void OnFaint()
    {
        CommandStop();
    }
}