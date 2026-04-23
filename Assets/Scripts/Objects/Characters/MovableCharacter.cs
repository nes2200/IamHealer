using Autodesk.Fbx;
using UnityEngine;

public class MovableCharacter : CharacterBase, IRunnable, IFunctionable
{
    [SerializeField] Animator anim;

    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;

    bool isMoving;

    public void RegistrationFunctions()
    {
        GameManager.OnPhysicsCharacter -= MovementUpdate;
        GameManager.OnPhysicsCharacter += MovementUpdate;
    }
    public void UnregistrationFunctions()
    {
        GameManager.OnUpdateCharacter -= MovementUpdate;
    }

    public void MovementUpdate(float deltaTime)
    {
        Vector3 originPosition = transform.position; //이동 전 위치 저장
        PhysicsUpdate(deltaTime); // 이동
        Vector3 positionDelta = transform.position - originPosition; //이동량 저장
        AnimationUpdate(positionDelta); //이동량에 따라 애니메이션 실행
    }

    public void AnimationUpdate(Vector3 moveDelta)
    {
        if (!anim) return;

        anim.SetFloat("MoveX", LookRotation.x);
        anim.SetFloat("MoveY", LookRotation.y);
        anim.SetFloat("MoveSpeed", moveDelta.magnitude / Time.fixedDeltaTime);
    }

    public void PhysicsUpdate(float deltaTime)
    {
        UpdateToDirection(deltaTime);
        UpdateToDestination(deltaTime);
    }

    public virtual float GetMoveSpeed() => 5.0f;
    public virtual float GetMoveSpeed(float deltaTime) => GetMoveSpeed() * deltaTime;

    public virtual void Translate(Vector3 delta)
    {
        transform.position += delta;
        //움직였을 때 해당 방향향을 바라보도록
        if(delta != Vector3.zero) _lookRotation = delta.normalized;
    }

    public void UpdateToDirection(float deltaTime)
    {
        if (targetDirection is null) return;
        
        float currentMoveSpeed = GetMoveSpeed(deltaTime);
        Translate(currentMoveSpeed * targetDirection.Value);
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

    public void MoveToDestination(Vector3 destination, float tolerance)
    {
        targetDirection = null;
        targetDestination = destination;
        targetTolerance = tolerance;
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
    }
}
