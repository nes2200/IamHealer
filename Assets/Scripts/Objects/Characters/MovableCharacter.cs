using UnityEngine;

public class MovableCharacter : CharacterBase, IRunnable, IFunctionable
{
    protected Vector3? targetDirection = null;
    protected Vector3? targetDestination = null;
    protected float targetTolerance;

    [SerializeField] float moveSpeed;

    public void RegistrationFunctions()
    {
        GameManager.OnPhysicsCharacter -= PhysicsUpdate;
        GameManager.OnPhysicsCharacter += PhysicsUpdate;
    }
    public void UnregistrationFunctions()
    {
        GameManager.OnUpdateCharacter -= PhysicsUpdate;
    }

    public void PhysicsUpdate(float deltaTime)
    {
        UpdateToDirection(deltaTime);
        UpdateToDestination(deltaTime);
    }
    public void UpdateToDirection(float deltaTime)
    {
        if (targetDirection is null) return;

        transform.position += deltaTime * moveSpeed * targetDirection.Value;
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
            float currentMoveSpeed = deltaTime * moveSpeed;
            //이동할 거리와 남은 거리를 비교하여, 더 짧은 거리를 가면 된다
            float resultMoveSpeed = Mathf.Min(currentMoveSpeed, distance);

            transform.position += resultMoveSpeed * currentMoveDirection;
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
