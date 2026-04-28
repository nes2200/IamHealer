using UnityEngine;

public class PlayerController3D : ControllerBase
{
    protected override void OnPossess(CharacterBase newCharacter)
    {
        base.OnPossess(newCharacter);
        InputManager.OnMove -= MoveToDirection;
        InputManager.OnMove += MoveToDirection;
    }

    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        base.OnUnpossess(oldCharacter);
        InputManager.OnMove -= MoveToDirection;
    }

    private void MoveToDirection(Vector2 value)
    {
        Vector3 direction = new Vector3(value.x, 0f, value.y);

        CommandMoveToDirection(direction);
    }

    //회전?
    //WS -> 앞뒤로 이동
    //AD -> 좌우 회전

}
