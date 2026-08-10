using System;
using UnityEngine;
using UnityEngine.AI;

public class MovementModule : CharacterModule, IRunnable
{
    [Header("이동 속도")]
    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float rotateSpeed = 2.0f;

    [Header("필수 부속품들")]
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] TargetingModule targetModule;

    //회전용
    Vector3? targetRotationDirection;

    public sealed override System.Type RegistrationType => typeof(MovementModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        GameManager.OnPhysicsCharacter -= MovementUpdate;
        GameManager.OnPhysicsCharacter += MovementUpdate;
        SetNavmeshAgent();

        targetModule.OnTargetScanned -= SetDestination;
        targetModule.OnTargetScanned += SetDestination;

        newOwner.OnFaint -= StopAllMovementByFaint;
        newOwner.OnFaint += StopAllMovementByFaint;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);

        GameManager.OnPhysicsCharacter -= MovementUpdate;
        targetModule.OnTargetScanned -= SetDestination;
        oldOwner.OnFaint -= StopAllMovementByFaint;
    }

    public void SetNavmeshAgent()
    {
        if (!navAgent) navAgent = GetComponent<NavMeshAgent>();

        navAgent.updateRotation = true;
        navAgent.speed = moveSpeed;
        navAgent.angularSpeed = rotateSpeed;
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

        Vector3 positionDelta = navAgent.velocity * deltaTime;
        Owner.MovementNotify(positionDelta);
    }

    // destination은 TargetingModule이 담당하고, 이 명령은 정지 허용 거리만 설정한다.
    public void MoveToDestination(Vector3 destination, float tolerance)
    {
        if (!navAgent) return;

        //새 이동 시작시 navMesh의 자동 회전과 충돌하지 않도록 해제하기
        targetRotationDirection = null;
        navAgent.stoppingDistance = Mathf.Max(0f, tolerance);
    }

    //회전 관련
    public void Rotate(Vector3 direction)
    {
        direction.y = 0f;

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
