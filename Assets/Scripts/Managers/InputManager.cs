using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

//이벤트
//      대리자
//플레이어가 할 일을 대신 해주고, 열려있는 창이 있다면 그 친구의 기능도 수행하고
//내가 신호주면 열결되어 있는 모든 애들이 한번에 기능을 수행하고 간다
public delegate void MouseMoveEvent(Vector2 screenPosition, Vector3 worldPosition);
public delegate void MouseButtonEvent(bool value, Vector2 screenPosition, Vector3 worldPosition);
public delegate void ButtonEvent(bool value);
public delegate void VectorEvent(Vector2 value);
public delegate void AxisEvent(float value);

//특정 클래스는 특정 컴포넌트와 함께 사용해야 한다
//특정 클래스가 다른 클래스를 Dependence, 의존하는 경우
//다른 클래스가 필요하다 -> Require
//대상 변수나 클래스 위쪽에다가 [이렇게] 내용을 넣는 것을 Attribute : 속성
[RequireComponent(typeof(PlayerInput))]
public class InputManager : ManagerBase
{
    //그냥 대리자는 누구나 등록하고 시전할 수 있지만
    //event 대리자는 누구나 등록하고 나만 시전할 수 있음
    public static event MouseButtonEvent OnMouseLeftButton;
    public static event MouseButtonEvent OnMouseRightButton;
    public static event MouseButtonEvent OnMouseWheelButton;
    public static event MouseMoveEvent   OnMouseMove;
    public static event ButtonEvent      OnCancel;
    public static event ButtonEvent      OnShowStatus;
    public static event VectorEvent      OnMove;
    public static event Action           OnAnyKey;


    PlayerInput targetInput;
    Dictionary<string, InputAction> actionDictionary = new();

    List<RaycastResult> cursorHitList = new();

    Vector2 cursorScreenPosition;
    Vector3 cursorWorldPosition;

    public bool is2D = true;

    bool canInput = true;
    public bool CanInput { get { return canInput; } set { CanInput = value; } }

    protected override IEnumerator Onconnected(GameManager newManager)
    {
        targetInput = GetComponent<PlayerInput>();

        LoadAllActions();
        InitializaAllActions();
        GameManager.OnUpdateManager -= UpdateEvent; //있으면 빼고, 없으면 아무일도 없고
        GameManager.OnUpdateManager += UpdateEvent;
        yield return null;

    }

    protected override void OnDisconnected()
    {
        GameManager.OnUpdateManager -= UpdateEvent;
    }

    public void UpdateEvent(float deltaTime)
    {
        RefreshGameObjectUnderCursor();
    }

    void RefreshGameObjectUnderCursor()
    {
        cursorHitList.Clear();
        if (is2D)
        {
            GameManager.Instance.Camera.GetRaycastResult2D(cursorScreenPosition, cursorHitList);
        }
        else
        {
            GameManager.Instance.Camera.GetRaycastResult3D(cursorScreenPosition, cursorHitList);
        }
    }

    public GameObject GetGameObjectUnderCursor()
    {
        if (cursorHitList.Count == 0) return null;

        return cursorHitList[0].gameObject;
    }

    void LoadAllActions()
    {
        foreach (InputAction currentAction in targetInput.actions)
        {
            actionDictionary.TryAdd(currentAction.name, currentAction);
        }
    }

    void InitializaAllActions()
    {
        if (actionDictionary == null || actionDictionary.Count == 0) return;

        InitializeAction("CursorPositionChanged",(context) => CursorPositionChanged(GetVector2Value(context)));
        InitializeAction("Move",                 (context) => OnMove            ?.Invoke(GetVector2Value(context)));

        InitializeAction("MouseLeftButtonDown",  (context) => OnMouseLeftButton ?.Invoke(true, cursorScreenPosition, cursorWorldPosition)) ;
        InitializeAction("MouseRightButtonDown", (context) => OnMouseRightButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition));
        InitializeAction("MouseLeftButtonUp",    (context) => OnMouseLeftButton ?.Invoke(false, cursorScreenPosition, cursorWorldPosition));
        InitializeAction("MouseRightButtonUp",   (context) => OnMouseRightButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition));
        
        InitializeAction("Cancel",               (context) => OnCancel          ?.Invoke(true));
        InitializeAction("ShowStatusButtonDown", (context) => OnShowStatus      ?.Invoke(true));
        InitializeAction("ShowStatusButtonUp",   (context) => OnShowStatus      ?.Invoke(false));   

        InitializeAction("MouseWheelButtonDown", (context) => OnMouseWheelButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition));   
        InitializeAction("MouseWheelButtonUp",   (context) => OnMouseWheelButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition));

        InitializeAction("AnyKey",               (context) => OnAnyKey          ?.Invoke());
    }

    void InitializeAction(string actionName, Action<InputAction.CallbackContext> actionMethod) 
    {
        if (actionDictionary == null) return;

        if (actionDictionary.TryGetValue(actionName, out InputAction cursorPositionChanged))
        {
            cursorPositionChanged.performed += actionMethod;
        }
    }

    Vector2 GetVector2Value(InputAction.CallbackContext context) => GetInputValue<Vector2>(context);

    T GetInputValue<T>(InputAction.CallbackContext context) where T : struct
    {
        if (context.valueType != typeof(T)) return default;
        return context.ReadValue<T>();
    }

    void CursorPositionChanged(Vector2 screenPosition)
    {
        //마우스의 화면상 실제 픽셀 위치
        //화면과 유티니간의 좌표가 다르다 -> 바꿔줘야 한다. -> 기준점이 필요
        //카메라를 기준으로 세상을 본다
        Vector3 worldPosition;

        if(is2D)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;
        }
        else
        {
            worldPosition = Vector3.zero;
        }
        cursorScreenPosition = screenPosition;
        cursorWorldPosition = worldPosition;

        OnMouseMove?.Invoke(screenPosition, worldPosition);
    }

    public void SetInputState(bool isEnabled)
    {
        if (isEnabled) targetInput.enabled = true;
        else targetInput.enabled = false;
    }
}
