using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using UnityEditor.Build;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

//이벤트
//      대리자
//플레이어가 할 일을 대신 해주고, 열려있는 창이 있다면 그 친구의 기능도 수행하고
//내가 신호주면 열결되어 있는 모든 애들이 한번에 기능을 수행하고 간다
public delegate void MouseDownEvent(Vector2 screenPosition, Vector3 position);
public delegate void MouseUpEvent(Vector2 screenPosition, Vector3 position);
public delegate void MouseMoveEvent(Vector2 screenPosition, Vector3 worldPosition);

//특정 클래스는 특정 컴포넌트와 함께 사용해야 한다
//특정 클래스가 다른 클래스를 Dependence, 의존하는 경우
//다른 클래스가 필요하다 -> Require
//대상 변수나 클래스 위쪽에다가 [이렇게] 내용을 넣는 것을 Attribute : 속성
[RequireComponent(typeof(PlayerInput))]
public class InputManager : ManagerBase
{
    //그냥 대리자는 누구나 등록하고 시전할 수 있지만
    //event 대리자는 누구나 등록하고 나만 시전할 수 있음
    public static event MouseDownEvent OnMouseLeftDown;
    public static event MouseDownEvent OnMouseRightDown;
    public static event MouseUpEvent   OnMouseLeftUp;
    public static event MouseUpEvent   OnMouseRightUp;
    public static event MouseMoveEvent OnMouseMove;

    PlayerInput targetInput;
    Dictionary<string, InputAction> actionDictionary = new();

    List<RaycastResult> cursorHitList = new();

    Vector2 cursorScreenPosition;
    Vector3 cursorWorldPosition;

    public bool is2D = true;

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
        GameManager.Instance.Camera.GetRaycastResult2D(cursorScreenPosition, cursorHitList);
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

        InitializeAction("CursorPositionChanged", CursorPositionChanged);
        InitializeAction("MouseLeftButtonDown", (context) => OnMouseLeftDown?.Invoke(cursorScreenPosition, cursorWorldPosition)) ;
        InitializeAction("MouseRightButtonDown", (context) => OnMouseRightDown?.Invoke(cursorScreenPosition, cursorWorldPosition));
        InitializeAction("MouseLeftButtonUp", (context) => OnMouseLeftUp?.Invoke(cursorScreenPosition, cursorWorldPosition));
        InitializeAction("MouseRightButtonUp", (context) => OnMouseRightUp?.Invoke(cursorScreenPosition, cursorWorldPosition));
    }

    void InitializeAction(string actionName, Action<InputAction.CallbackContext> actionMethod) 
    {
        if (actionDictionary == null) return;

        if (actionDictionary.TryGetValue(actionName, out InputAction cursorPositionChanged))
        {
            cursorPositionChanged.performed += actionMethod;
        }
    }

    void CursorPositionChanged(InputAction.CallbackContext context)
    {
        //마우스의 화면상 실제 픽셀 위치
        Vector2 screenPosition = context.ReadValue<Vector2>();
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

    
}
