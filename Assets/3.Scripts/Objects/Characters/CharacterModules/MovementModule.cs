using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MovementModule : CharacterModule, IRunnable
{
    [Header("이동 속도")]
    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float rotateSpeed = 2.0f;

    [Header("필수 부속품들")]
    [SerializeField] NavMeshAgent navAgent;

    //회전용
    Vector3? targetRotationDirection;


    public sealed override System.Type RegistrationType => typeof(MovementModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        GameManager.OnPhysicsCharacter -= MovementUpdate;
        GameManager.OnPhysicsCharacter += MovementUpdate;
        SetNavmeshAgent();


        newOwner.OnFaint -= StopAllMovementByFaint;
        newOwner.OnFaint += StopAllMovementByFaint;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);

        GameManager.OnPhysicsCharacter -= MovementUpdate;
        oldOwner.OnFaint -= StopAllMovementByFaint;
    }

    public void SetNavmeshAgent()
    {
        if (!navAgent) navAgent = GetComponent<NavMeshAgent>();

        navAgent.updateRotation = true;
        navAgent.speed = moveSpeed;
        navAgent.angularSpeed = rotateSpeed;

        //우선순위 변경으로 양보하기 
        int priorityOffset = (Owner.GetInstanceID() & int.MaxValue) % 11;


        navAgent.avoidancePriority = 45 + priorityOffset;
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
    }

    // 실제 목적지는 TargetingModule이 스캔한 위치로 계속 갱신한다.
    public void SetDestination(Vector3 targetPosition)
    {
        if (!navAgent || !navAgent.isOnNavMesh) return;

        navAgent.SetDestination(targetPosition);
    }

    public void MovementUpdate(float deltaTime)
    {
        if (!navAgent || !navAgent.isOnNavMesh) return;

        UpdateRotation(deltaTime);

        if(!navAgent.isStopped && !navAgent.pathPending && navAgent.hasPath)
        {
            if(navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                StopMovement();
            }
            else if (navAgent.desiredVelocity.sqrMagnitude > 0.001f)
            {
                //navMesh가 계산한 회피 방향은 사용하되 속도는 고정
                Vector3 direction = navAgent.desiredVelocity.normalized;
                navAgent.velocity = direction * moveSpeed;
            }
        }
        

            Vector3 positionDelta = navAgent.velocity * deltaTime;
        Owner.MovementNotify(positionDelta);
    }

    // destination은 TargetingModule이 담당하고, 이 명령은 정지 허용 거리만 설정한다.
    public void MoveToDestination(Vector3 destination, float tolerance)
    {
        if (!navAgent || !navAgent.isOnNavMesh) return;

        //새 이동 시작시 navMesh의 자동 회전과 충돌하지 않도록 해제하기
        targetRotationDirection = null;
        navAgent.updateRotation = true;

        navAgent.isStopped = false;
        navAgent.stoppingDistance = Mathf.Max(0f, tolerance);
        navAgent.SetDestination(destination);
    }

    //회전 관련
    public void Rotate(Vector3 direction)
    {
        direction.y = 0f;
        if (navAgent) navAgent.updateRotation = false;
        targetRotationDirection = direction.sqrMagnitude > Mathf.Epsilon ? direction.normalized : null;
    }
    private void UpdateRotation(float deltaTime)
    {
        if (targetRotationDirection is null) return;

        Quaternion targetRotation = Quaternion.LookRotation(targetRotationDirection.Value);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * deltaTime);
    }

    // 방향 기반 물리 이동은 LegacyPhysicsMovementModule.cs.txt에 보관한다.
    public void MoveToDirection(Vector3 direction) { }

    public void StopMovement()
    {
        if (!navAgent || !navAgent.isOnNavMesh) return;
        if (navAgent.isStopped) return;

        navAgent.isStopped = true;
        navAgent.ResetPath();
    }

    public void Move(Vector3 direction) { }

    public void StopAllMovementByFaint()
    {
        StopMovement();
        GameManager.OnPhysicsCharacter -= MovementUpdate;

        if (navAgent) navAgent.enabled = false;
    }
}
