using UnityEngine;

public class HostileAIController : AIController 
{
    [SerializeField] Transform _hostileGroupParent;
    public Transform HostileGroupParent => _hostileGroupParent;

    float scanCooltime; //스캔 쿨타임
    float scanInterval = 0.5f; //스캔 인터벌. 한번에 모든 유닛이 스캔하지 않도록 하기 위해서
    bool canScan;

    protected override void OnPossess(CharacterBase newCharacter)
    {
        GameManager.OnUpdateController -= Think;
        GameManager.OnUpdateController += Think;

        //초기 쿨타임 세팅
        scanCooltime = Random.Range(0.1f, scanInterval);
    }
    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        GameManager.OnUpdateController -= Think;
    }

    protected override void Think(float deltaTime)
    {
        //스캔 쿨타임이면 쿨타임 돌리기
        //스캔 가능하면 스캔 돌리기
        if (!canScan)
        {
            ScanCooltimeUpdate(deltaTime);
        }
        else
        {
            SearchFocusTarget();
        }

        //목표가 없어? 그럼 여기서 끝. 가만히 있어
        if (!FocusTarget) return;
        
        //때리던지 움직이든지 해라
        AttackOrMoveCheck();
    }

    //가장 가까운 적 찾기
    protected override void SearchFocusTarget()
    {
        //안전장치
        if (!HostileGroupParent || HostileGroupParent.childCount == 0)
        {
            SetFocusTarget(null);
            return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;
        Vector3 currentPosition = transform.position;

        foreach(Transform target in HostileGroupParent)
        {
            //나중에 여기다가 target이 죽었는지 살았는지 체크하는 과정 넣어야 함!!!
            //개발중이라 아직 없지만 꼭 넣어라!!!!!!!!!!!!!!!!!!!!!

            float distance = (target.position - currentPosition).sqrMagnitude;

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        SetFocusTarget(closestTarget.gameObject);
        canScan = false;
    }

    protected void ScanCooltimeUpdate(float deltaTime)
    {
        if (canScan) return;

        scanCooltime -= deltaTime;
        if(scanCooltime <= 0f)
        {
            canScan = true;
            //모든 유닛이 한번에 스캔하여 갑자기 렉걸리는 사태를 막기위해
            scanCooltime = Random.Range(0.1f, scanInterval);
        }
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