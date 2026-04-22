using UnityEngine;

public class ControllerBase : MonoBehaviour, IFunctionable
{
    CharacterBase _character;
    public CharacterBase Character => _character;

    public virtual void RegistrationFunctions()
    {
        Possess(GetComponent<CharacterBase>());
    }
    public virtual void UnregistrationFunctions()
    {
        Unpossess();
    }

    protected virtual void OnPossess(CharacterBase newCharacter) { }
    public void Possess(CharacterBase newCharacter)
    {
        if (!newCharacter) return;
        ControllerBase result = newCharacter.Possessed(this);
        if (result == this) 
        {
            _character = newCharacter;
            OnPossess(newCharacter); 
        }
    }
    protected virtual void OnUnpossess(CharacterBase oldCharacter) { }
    public void Unpossess()
    {
        if (Character)
        {
            //캐릭터에 들어있는게 '나'인게 맞는지 확인
            if (Character.Unpossessed(this))
            {
                OnUnpossess(Character);
            }
        }
        _character = null;
    }

    public void CommandMoveToDirection(Vector3 direction)
    {
        if (Character is IRunnable target) target.MoveToDirection(direction); 
    }
    public void CommandMoveToDestination(Vector3 destination, float tolerance)
    {
        if (Character is IRunnable target) target.MoveToDestination(destination, tolerance);
    }
    public void CommandStop()
    {
        if (Character is IRunnable target) target.StopMovement();
    }
}