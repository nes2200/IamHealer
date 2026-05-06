using Unity.Hierarchy;
using UnityEngine;

public class MovementModule : CharacterModule, IRunnable
{
    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected Vector3? targetRotation = null;
    protected float targetTolerance;
    protected float mainColliderRadius;

    [SerializeField]float moveSpeed = 2.0f;
    [SerializeField]float rotateSpeed = 2.0f;

    //이런 거대한 모듈을 만들 때에 한 번 "대분류"로 분류하기
    public sealed override System.Type RegistrationType => typeof(MovementModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        GameManager.OnPhysicsCharacter -= MovementUpdate;
        GameManager.OnPhysicsCharacter += MovementUpdate;

        newOwner.OnFaint -= StopAllMovementByFaint;
        newOwner.OnFaint += StopAllMovementByFaint;
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        GameManager.OnPhysicsCharacter -= MovementUpdate;

        oldOwner.OnFaint -= StopAllMovementByFaint;
    }

    public void MovementUpdate(float deltaTime)
    {
        Vector3 originPosition = transform.position; //이동 전 위치 저장
        PhysicsUpdate(deltaTime); // 이동
        Vector3 positionDelta = transform.position - originPosition; //이동량 저장
        Owner.MovementNotify(positionDelta); //이동량에 따라 애니메이션 실행

        //Vector3? moveDelta = targetDirection.Value + targetRotation.Value;
        //PhysicsUpdate(deltaTime);
        //Owner.MovementNotify(moveDelta.Value.normalized);
    }
    
    public void PhysicsUpdate(float deltaTime)
    {
        //UpdateToDirection(deltaTime);
        UpdateToDestination(deltaTime);
        UpdateMove(deltaTime);
        UpdateRotate(deltaTime);
    }

    public virtual float GetMoveSpeed() => moveSpeed;
    public virtual float GetMoveSpeed(float deltaTime) => GetMoveSpeed() * deltaTime;
    public virtual float GetRotateSpeed() => rotateSpeed;
    public virtual float GetRotateSpeed(float deltTime) => GetRotateSpeed() * deltTime;

    public virtual void Translate(Vector3 delta)
    {
        transform.position += delta;
    }

    public void UpdateToDirection(float deltaTime)
    {
        if (targetDirection is null) return;

        Vector3 localDirection = transform.TransformDirection(targetDirection.Value).normalized;

        float currentMoveSpeed = GetMoveSpeed(deltaTime);
        Translate(currentMoveSpeed * localDirection);
    }
    public void UpdateToDestination(float deltaTime)
    {
        if (targetDestination is null) return;
        //해당 위치로 가는 방향
        Vector3 currentMoveDirection = (targetDestination.Value - transform.position);
        //얼마나 남았나 거리 측정
        float distance = currentMoveDirection.magnitude;

        //거리가 인정 범위 밖에 있다
        if (distance >= targetTolerance)
        {
            //방향 잡기
            currentMoveDirection.Normalize();

            //한 번 이동할 때 거리
            float currentMoveSpeed = GetMoveSpeed(deltaTime);
            //이동할 거리와 남은 거리를 비교하여, 더 짧은 거리를 가면 된다
            float resultMoveSpeed = Mathf.Min(currentMoveSpeed, distance);

            Translate(resultMoveSpeed * currentMoveDirection);
        }
    }

    public void UpdateRotate(float deltaTime)
    {
        if(targetRotation is null || targetRotation == Vector3.zero) return;

        //받아온 방향을 로컬로 바꾸기
        //Vector3 localRotation = transform.TransformDirection(targetRotation.Value).normalized;
        //로컬 방향을 가지고 회전 생성
        Quaternion targetLocalRotation = Quaternion.LookRotation(targetRotation.Value);
        //받아온 회전을 부드럽게 처리하여 돌리기
        transform.rotation = Quaternion.Slerp(transform.rotation, targetLocalRotation, GetRotateSpeed(deltaTime));
    }
    public void UpdateMove(float deltaTime)
    {
        if (targetDirection is null) return;

        Vector3 localDirection = transform.TransformDirection(targetDirection.Value).normalized;

        float currentMoveSpeed = GetMoveSpeed(deltaTime);
        Translate(currentMoveSpeed * localDirection);
    }

    public void MoveToDestination(Vector3 destination, float tolerance)
    {
        targetDirection = null;
        targetDestination = destination;
        targetRotation = destination - transform.position;
        targetTolerance = tolerance;

        //목적지를 받았다
        //목적지 방향으로 rotate 해야한다.
        //목적지까지 move 해야한다.
    }
    public void MoveToDirection(Vector3 direction)
    {
        targetDestination = null;
        targetDirection = direction.normalized;
    }
    public void StopMovement()
    {
        targetDirection = null;
        targetDestination = null;
        targetRotation = null;
    }
    public void Rotate(Vector3 direction)
    {
        targetRotation = direction;
    }
    public void Move(Vector3 direction)
    {
        targetDirection = direction;
    }

    public void StopAllMovementByFaint()
    {
        StopMovement();
        GameManager.OnPhysicsCharacter -= MovementUpdate;
    }

    protected void SetMainColliderRadius()
    {
        mainColliderRadius = Owner.GetModule<AnimationModule>().MainCollider.radius;
    }
    public float GetMainColliderRadius()
    {
         return mainColliderRadius;
    }
}

//MovementModule의 기능을 새로 만들기 -> 현재는 2D를 베이스로 하는 이동방식. 3D에서 XZ축을 이용한 이동방식을 새로 구현해야 한다. Y는 회전용
//Move() -> 이동은 앞뒤로 하게될 것
//Rotate() -> 회전
//이때, 두 개의 함수에서 이동량과 회전량을 받아 애니메이션 실행

//이동하기 -> 앞뒤로만. move에는 ws만 쓴다
//회전하기 -> 좌우로만. rotation에는 ad만 쓴다

//input에서 move를 ws로 두고, rotation을 새로 만들어서 ad를 받기
//irunnable에서 Rotate를 만들기 -> 하려니까 모든 상속 스크립트에서 만들어야함.
//기존 코드를 수정해서 쓸까>?