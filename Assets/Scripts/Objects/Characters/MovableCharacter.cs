using UnityEngine;

public class MovableCharacter : CharacterBase, IRunnable, IFunctionable
{
    protected Vector3 targetDestination;
    protected float targetTolerance;

    private void Start()
    {
        RegistrationFunctions();
    }

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
        //해당 위치로 가는 방향
        Vector3 currentMoveDirection = (targetDestination - transform.position);
        //얼마나 남았나 거리 측정
        float distance = currentMoveDirection.magnitude;

        //거리가 인정 범위 밖에 있다
        if (distance >= targetTolerance)
        {
            //방향 잡기
            currentMoveDirection.Normalize();
            //거리 = 시간 * 속력
            transform.position += deltaTime * 5.0f * currentMoveDirection;
        }
    }

    public void MoveToDestination(Vector3 destination, float tolerance)
    {
        targetDestination = destination;
        targetTolerance = tolerance;
    }
    public void MoveToDirection(Vector3 direction)
    {

    }
    public void StopMovement()
    {

    }

    
}
