using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    public GameObject square;
    void Start()
    {
        InputManager.OnMouseLeftUp += CreatToMouse;
        InputManager.OnMouseRightDown += DestroyOnMouse;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }
    void CreatToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        Instantiate(DataManager.LoadDataFile<GameObject>("Square 7"), worldPosition, Quaternion.identity);
    }
    void DestroyOnMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        Debug.Log(GameManager.Instance.Input.GetGameObjectUnderCursor()); 
    }
}
