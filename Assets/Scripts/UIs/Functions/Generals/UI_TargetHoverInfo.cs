using System;
using UnityEngine;

public class UI_TargetHoverInfo : OpenableUIBase
{
    [SerializeField] Vector2 shiftedPosition;

    [SerializeField] TMPro.TextMeshProUGUI nameText;

    CharacterBase target;

    //초기화
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        InputManager.OnMouseHover += HoverInfoChange;
        InputManager.OnMouseMove -= MoveToMouse;
        InputManager.OnMouseMove += MoveToMouse;
    }
    //해제
    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        InputManager.OnMouseMove -= MoveToMouse;
    }
   
    private void HoverInfoChange(GameObject newTarget, GameObject oldTarget)
    {
        CharacterBase asCharacter = newTarget?.GetComponent<CharacterBase>();

        if (asCharacter) 
        {
            nameText.text = asCharacter.DisplayName;
            Open();
        } 
        else Close();

        target = asCharacter;
    }
    private void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = screenPosition + shiftedPosition;
    }
}
