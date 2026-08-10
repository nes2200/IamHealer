using UnityEngine;

public class HostileAIController : AIController 
{
    TargetingModule targetModule;
    AttackModule atkModule;
    HitPointModule targetHPModule;
    float targetRadius;

    TeamEliminateNotifier teamEliminateNotifier;

    [Header("Attack Distance")]
    [SerializeField] float attackDistance = 0.5f;
    [SerializeField] float approachMargin = 0.05f;
    
    protected override void OnPossess(CharacterBase newCharacter)
    {
        GameManager.OnUpdateController -= Think;
        GameManager.OnUpdateController += Think;

        newCharacter.OnFaint -= OnFaint;
        newCharacter.OnFaint += OnFaint;

        targetModule = Character.GetModule<TargetingModule>();
        atkModule = Character.GetModule<AttackModule>();
        teamEliminateNotifier = GetComponentInParent<TeamEliminateNotifier>();
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
            //그리고 강제 스캔 돌리기
            targetModule.ForceScanReady();
        }

        //스캔 주기마다 스캔 시도
        if (targetModule.TryGetNewTarget(deltaTime, out GameObject newTarget))
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
        if (!FocusTarget || targetHPModule?.IsEmpty == true)
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

        //상대와 나의 크기 값
        float combineRadius = Character.Status.colliderRadius + targetRadius;
        //상대와 나의 거리
        float centerDistance = Vector3.Distance(transform.position, FocusTarget.transform.position);
        //상대와 나의 거리에서 크기를 뺀 실제 거리값
        float surfaceDistance = centerDistance - combineRadius;

        //만약 여유공간 만큼 들어왔다면 공격, 아니면 이동
        if (surfaceDistance <= attackDistance)
        {
            // 이동 경로를 끊어도 적을 향한 회전은 계속 갱신한다.
            CommandStop();
            CommandRotateToDirection(FocusTarget.transform.position - transform.position);
            TryAttack();
            return;
        }

        //NavMesh의 목적지는 대상의 중심이므로 반지름을 다시 더해서 정지거리를 만들어준다
        float stoppingDistance = combineRadius + attackDistance - approachMargin;
        CommandMoveToDestination(FocusTarget.transform.position, stoppingDistance);
        
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
        teamEliminateNotifier.TeamEliminateCheck();
    }
}
