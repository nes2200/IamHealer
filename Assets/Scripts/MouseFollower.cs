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
        InputManager.OnCancel += CancelButton;
        InputManager.OnShowStatus += StatusButton;
        InputManager.OnMouseWheelButton += WheelButton;
        InputManager.OnMove += MoveButton;
    }

    public void UnregistrationFunctions()
    {
        InputManager.OnCancel -= CancelButton;
        InputManager.OnShowStatus -= StatusButton;
        InputManager.OnMouseWheelButton -= WheelButton;
        InputManager.OnMove -= MoveButton;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }
    void CreatToMouse(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        GameObject inst = ObjectManager.CreateObject("NemoMan", worldPosition);
    }
    void DestroyOnMouse(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        ObjectManager.DestroyObject(GameManager.Input.GetGameObjectUnderCursor());
    }
    void CancelButton(bool value)
    {
        UIManager.ClaimPopUp("취소", "ESC", "취소");
    }
    void StatusButton(bool value)
    {
        UIManager.ClaimPopUp("취소", "Tab", "취소");
    }
    void WheelButton(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        string press = "";
        press = value ? "Wheel 누름" : "Wheel 뗌";

        UIManager.ClaimPopUp("취소", press, "취소");
    }
    void MoveButton(Vector2 value)
    {
        Debug.Log(value);
    }
}
