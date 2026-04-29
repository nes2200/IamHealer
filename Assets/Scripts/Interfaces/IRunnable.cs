using UnityEngine;

public interface IRunnable
{
    public void MoveToDestination(Vector3 destination, float tolerance);
    public void MoveToDirection(Vector3 direction);
    public void Rotate(Vector3 direction);
    public void Move(Vector3 direction);
    public void StopMovement();

}
