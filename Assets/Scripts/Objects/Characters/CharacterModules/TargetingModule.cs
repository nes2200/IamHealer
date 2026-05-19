using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetingModule : CharacterModule
{
    [SerializeField] Transform _hostileGroupParent;
    public Transform HostileGroupParent => _hostileGroupParent;
    private List<CharacterBase> hostileCharacters = new List<CharacterBase>(100);

    float scanCooltime; //스캔 쿨타임
    float scanInterval = 0.5f; //스캔 인터벌. 한번에 모든 유닛이 스캔하지 않도록 하기 위해서
    bool _canScan;
    public bool CanScan => _canScan;

    public override Type RegistrationType => typeof(TargetingModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        //초기 쿨타임 세팅
        scanCooltime = UnityEngine.Random.Range(0.1f, scanInterval);
        CacheHostileCharacters();
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
    }

    //미리 적 유닛들의 CharacterBase를 캐싱하는 함수. 매 프레임마다 GetComponent하는 것을 방지.
    private void CacheHostileCharacters()
    {
        hostileCharacters.Clear();

        if (!HostileGroupParent) return;

        foreach(Transform target in HostileGroupParent)
        {
            CharacterBase targetCharacter = target.GetComponent<CharacterBase>();
            if (targetCharacter)
            {
                hostileCharacters.Add(targetCharacter);
            }
        }
    }

    //적 유닛들의 부모가 바뀌었을 때, 혹은 새로 등록할 때
    public void SetHostileGroupPanrets(Transform newHostileParent)
    {
        //부모 바꿔주고 새로 캐싱하기
        _hostileGroupParent = newHostileParent;
        CacheHostileCharacters();
    }

    //스캔 시도하기
    public bool TryGetNewTarget(float deltaTime, out GameObject newTarget)
    {
        newTarget = null;

        //스캔 불가능하면 쿨타임만 돌리기
        if (!CanScan)
        {
            ScanCooltimeUpdate(deltaTime);
            return false;
        }

        //스캔 돌렸는데 돌린게 null이 아니다? 그럼 그걸 out에 넣어준다
        GameObject target = ScanClosestTarget();
        if (target)
        {
            newTarget = target;
            return true;
        }

        //스캔을 돌렸는데, 돌린게 null이기 때문에 false를 반환한다
        return false;
    }

    public GameObject ScanClosestTarget()
    {
        //안전장치
        if (hostileCharacters.Count == 0)
        {
            return null;
        }

        float closestDistance = Mathf.Infinity;
        CharacterBase closestTarget = null;
        Vector3 currentPosition = transform.position;

        for(int i = 0; i < hostileCharacters.Count; i++)
        {
            CharacterBase target = hostileCharacters[i];

            //죽은 상태면 continue
            if (!target.IsAlive)
            {
                continue;
            }

            float distance = (target.transform.position - currentPosition).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        _canScan = false;
        return closestTarget?.gameObject;
    }

    protected void ScanCooltimeUpdate(float deltaTime)
    {
        if (_canScan) return;

        scanCooltime -= deltaTime;
        if (scanCooltime <= 0f)
        {
            _canScan = true;
            //모든 유닛이 한번에 스캔하여 갑자기 렉걸리는 사태를 막기위해
            scanCooltime = UnityEngine.Random.Range(0.1f, scanInterval);
        }
    }

    public void ForceScanReady()
    {
        _canScan = true;
        scanCooltime = 0f;
    }
}
