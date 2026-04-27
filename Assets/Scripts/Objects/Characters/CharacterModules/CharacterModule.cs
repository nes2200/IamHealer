using UnityEngine;

public class CharacterModule : MonoBehaviour
{
    //나의 "대분류"를 저장하는 방법
    public virtual System.Type RegistrationType => typeof(CharacterModule);

    CharacterBase _owner;
    public CharacterBase Owner => _owner;

    public virtual void OnRegistration(CharacterBase newOwner)
    {
        _owner = newOwner;
    }
    public virtual void OnUnregistration(CharacterBase oldOwner)
    {
        _owner = oldOwner;
    }
}
