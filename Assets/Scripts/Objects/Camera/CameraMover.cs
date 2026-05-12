using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraMover : MonoBehaviour
{
    Vector2 moveDirection;
    float rotateDirection;

    float moveSpeed = 10f;
    float rotateSpeed = 20f;

    bool _isRotating = false;
    public bool IsRotating { get { return _isRotating; } set { _isRotating = value; } }

    public void SetCameraMover()
    {
        GameManager.OnUpdateCamera -= CameraTransformUpdate;
        GameManager.OnUpdateCamera += CameraTransformUpdate;
    }
    public void RemoveCameraMover()
    {

        GameManager.OnUpdateCamera -= CameraTransformUpdate;
    }

    void CameraTransformUpdate(float deltaTime)
    {
        MovementUpdate(deltaTime);
        RotateUpdate(deltaTime);
    }
    public void MovementUpdate(float deltaTime)
    {
        if(moveDirection == Vector2.zero) return;

        Vector3 localDirection = new Vector3(moveDirection.x, 0f, moveDirection.y);
        transform.Translate(localDirection * moveSpeed * deltaTime);
    }
    public void RotateUpdate(float deltaTime)
    {
        if (!IsRotating) return;


    }

    public void SetMoveDireciton(Vector2 direction)
    {
        moveDirection = direction;
    }
    public void SetRotateDirection(float value)
    {
       
    }
    
}
