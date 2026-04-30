using System;
using UnityEngine;

public struct AttackInfo
{
    public GameObject target;
    public ControllerBase instigator;
    public int damageAmount;
}

//이 모듈은 '공격'을 담당하는 모듈.
//hit과 같은 '맞는 역할'은 HitPointModule이 담당한다.
public class AttackModule : CharacterModule
{
    public override Type RegistrationType => typeof(AttackModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
    }
}