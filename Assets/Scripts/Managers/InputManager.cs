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
    public static event VectorEvent      OnRotate;
    public static event Action           OnAnyKey;


    PlayerInput targetInput;
    Dictionary<string, InputAction> actionDictionary = new();

    List<RaycastResult> cursorHitList = new();

    GameObject cursorHoverObject;
    Vector2 cursorScreenPosition;
    Vector3 cursorWorldPosition;

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
        RefreshGameObjectUnderCursor(cursorScreenPosition);
    }

    void RefreshGameObjectUnderCursor(Vector2 screenPosition)
    {
        cursorHitList.Clear();
        GameManager.Instance.Camera.GetRaycastResult(screenPosition, cursorHitList);

        //마우스의 화면상 실제 픽셀 위치
        //화면과 유티니간의 좌표가 다르다 -> 바꿔줘야 한다. -> 기준점이 필요
        //카메라를 기준으로 세상을 본다
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GameObject closestObject = null;

        //마우스에 닿을 수 있는 물체는 뭐가 있을까
        //UI 2D 3D
        //맨 첫번째에 있는 친구가 보통 UI일 가능성이 높다
        //제일 첫번째 친구가 GraphicRaycaster에 의해서 선별될 경우 -> 첫번째 친구가 UI구나
        if(cursorHitList.Count > 0 && cursorHitList[0].element != null)
        {
            closestObject = cursorHitList[0].gameObject;
        }
        if (GameManager.is2D)
        {
            worldPosition.z = 0f;

            //Order in Layer는 2byte 자료형
            //-32768 ~ 32767 까지만 저장이 가능
            //Layer를 100000배 해버리고 Order를 더하주면
            //Layer가 1일때 67232 ~ 132767 사이의 값이 무조건 나오기 때문에
            //밑이나 위의 레이어, 그러니까 0이나 2의 레이어는 침범할 가능성이 없다
            float GetValue(RaycastResult target)
            {
                return target.sortingOrder + target.sortingLayer * 100000;
            }
            RaycastResult nearest = cursorHitList.GetMaximum<RaycastResult>(GetValue);
            closestObject = nearest.gameObject;
            worldPosition = nearest.worldPosition;
        }
        else
        {
            //함수 내부에서 함수 만들기
            float GetDistance(RaycastResult target)
            {
                return target.distance;
            }

            //cursorHitList.GetMinimum<RaycastResult>((target) => target.distance);
            RaycastResult nearest = cursorHitList.GetMinimum<RaycastResult>(GetDistance);
            closestObject = nearest.gameObject;
            worldPosition = nearest.worldPosition;
        }

            //마우스가 닿은 대상의 표면 위치 중에서 가장 화면에서 가까운 대상 찾기
        float minDistance = float.MaxValue;
        Vector3 contactPosition = worldPosition;
        foreach (RaycastResult currentResult in cursorHitList)
        {
            float currentDistance = currentResult.distance;
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestObject = currentResult.gameObject;
                contactPosition = currentResult.worldPosition;
            }
        }

        cursorScreenPosition = screenPosition;
        cursorWorldPosition = worldPosition;
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
        InitializeAction("Move"                 ,(context) => OnMove            ?.Invoke(GetVector2Value(context))
                                                ,(context) => OnMove            ?.Invoke(Vector2.zero));
        InitializeAction("Rotate"               ,(context) => OnRotate          ?.Invoke(GetVector2Value(context))
                                                ,(context) => OnRotate          ?.Invoke(Vector2.zero));

        InitializeAction("MouseLeftButton"      ,(context) => OnMouseLeftButton ?.Invoke(true, cursorScreenPosition, cursorWorldPosition)
                                                ,(context) => OnMouseLeftButton ?.Invoke(false, cursorScreenPosition, cursorWorldPosition));

        InitializeAction("MouseRightButton"     ,(context) => OnMouseRightButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition)
                                                ,(context) => OnMouseRightButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition));
        
        InitializeAction("ShowStatusButton"     ,(context) => OnShowStatus      ?.Invoke(true)
                                                ,(context) => OnShowStatus       ?.Invoke(false));   

        InitializeAction("MouseWheelButton"     ,(context) => OnMouseWheelButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition)
                                                ,(context) => OnMouseWheelButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition));

        InitializeAction("Cancel",               (context) => OnCancel          ?.Invoke(true));
        InitializeAction("AnyKey",               (context) => OnAnyKey          ?.Invoke());
    }

    void InitializeAction(string actionName, Action<InputAction.CallbackContext> actionMethod, Action<InputAction.CallbackContext> cancelMethod = null) 
    {
        if (actionDictionary == null) return;

        if (actionDictionary.TryGetValue(actionName, out InputAction currentInput))
        {
            //발동될 때 할일
            if(actionMethod is not null)currentInput.performed += actionMethod;
            //취소될 때 할일
            if(cancelMethod is not null)currentInput.canceled += cancelMethod;
            //키가 눌렸을 때
            //currentInput.started
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
        RefreshGameObjectUnderCursor(screenPosition); //새로고침

        OnMouseMove?.Invoke(cursorScreenPosition, cursorWorldPosition);
    }

    public void SetInputState(bool isEnabled)
    {
        if (isEnabled) targetInput.enabled = true;
        else targetInput.enabled = false;
    }
}
