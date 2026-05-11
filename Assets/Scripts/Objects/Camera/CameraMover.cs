using UnityEngine;

public class CameraMover : MonoBehaviour
{
    Vector2 moveDirection;

    float moveSpeed = 2f;
    float rotateSpeed = 2f;

    public void SetCameraMover()
    {z`
        GameManager.OnUpdateCamera -= MovementUpdate;
        GameManager.OnUpdateCamera += MovementUpdate;
    }
    public void RemoveCameraMover()
    {

        GameManager.OnUpdateCamera -= MovementUpdate;
    }

    public void MovementUpdate(float deltaTime)
    {
        if(moveDirection == Vector2.zero) return;

        Vector2 localDirection = transform.InverseTransformDirection(moveDirection);
        transform.position += (Vector3)localDirection * deltaTime * moveSpeed;
    }

    public void SetMoveDireciton(Vector2 direction)
    {
        moveDirection = direction;
    }
}
