using UnityEngine;

public class CameraMover : MonoBehaviour
{
    Vector2 moveDirection;

    float moveSpeed = 5f;
    float rotateSpeed = 2f;

    public void SetCameraMover()
    {
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

        Vector3 localDirection = new Vector3(moveDirection.x, 0f, moveDirection.y);
        transform.Translate(localDirection * moveSpeed * deltaTime);
    }

    public void SetMoveDireciton(Vector2 direction)
    {
        moveDirection = direction;
    }
}
