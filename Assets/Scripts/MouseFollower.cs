using UnityEngine;

public class MouseFollower : MonoBehaviour, IFunctionable
{
    public GameObject square;

    private void Start()
    {
        RegistrationFunctions();
    }
    private void OnDestroy()
    {
        UnregistrationFunctions();
    } 

    public void RegistrationFunctions()
    {
        InputManager.OnMouseLeftUp += CreatToMouse;
        InputManager.OnMouseRightDown += DestroyOnMouse;
    }

    public void UnregistrationFunctions()
    {
        InputManager.OnMouseLeftUp -= CreatToMouse;
        InputManager.OnMouseRightDown -= DestroyOnMouse;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }
    void CreatToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        GameObject inst =  ObjectManager.CreateObject(DataManager.LoadDataFile<GameObject>("Square 7"));
        inst.transform.position = worldPosition;
    }
    void DestroyOnMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        ObjectManager.DestroyObject(GameManager.Instance.Input.GetGameObjectUnderCursor()); 
    }
}
