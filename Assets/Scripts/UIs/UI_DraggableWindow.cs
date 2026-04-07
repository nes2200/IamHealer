using UnityEngine;
using UnityEngine.EventSystems;

public delegate void DragStartEvent(UI_DraggableWindow dragTarget, Vector2 startPosition);

public class UI_DraggableWindow : UIBase, IPointerDownHandler
{
    public event DragStartEvent OnDragStart;

    //드래그하면 어떤 트랜스폼을 움직여야 할까>?
    [SerializeField] RectTransform rootTransform;

    /// <summary> 마지막으로 수신받은 마우스 위치 </summary>
    Vector2 currentScreenPosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDragStart?.Invoke(this, eventData.position);
    }

    public void SetMouseStartPosition(Vector2 screenPosition)
    {
        currentScreenPosition = screenPosition;
    }
    public void SetMouseCurrentPosition(Vector2 screenPosition)
    {
        Vector2 screenDelta = screenPosition - currentScreenPosition;
        Rect rootRect = rootTransform.rect; 
        //바꾸고 나서 얼마나 튀어나갔는가를 확인해보기
        rootRect.position += screenDelta;
        //튀어나온걸 보정해주는 값을 InversedAABB가 돌려주니까 보정해주는 위치만큼 이동을 자제한다
        screenDelta += rootRect.InversedAABB(UIManager.UIBoundary);

        Vector3 positionDelta = (Vector3)screenDelta;

        if(UIManager.UIScale > 0f) positionDelta /= UIManager.UIScale;

        rootTransform.localPosition += positionDelta;
        currentScreenPosition = screenPosition;
        
    }

}
