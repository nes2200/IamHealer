using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    public GameObject square;
    void Start()
    {
        InputManager.OnMouseMove += MoveToMouse;
        InputManager.OnMouseLeftDown += MakeCopy;
    }

    void MoveToMouse(Vector2 screenPos, Vector3 worldPos)
    {

        transform.position = worldPos;
    }
    void MakeCopy(Vector3 worldPos)
    {
        Instantiate(square).transform.position = worldPos;
        
    }
}
