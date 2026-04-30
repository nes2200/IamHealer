using System;
using UnityEngine;

public struct DamageStruct
{
    public GameObject from;
    public ControllerBase instigator;
    public int damageAmount;
    //public bool critical;
    //public ElemantalType damageType;
}
public struct RestoreStruct
{
    public GameObject from;
    public ControllerBase instigator;
    public int restoreAmount;
}

public class HitPointModule : CharacterModule
{
    protected FillValue fill; /*= new FillValue(20, 20);*/
    public override Type RegistrationType => typeof(HitPointModule);

    public int Max => fill.Max;
    public int Min => fill.Min;
    public bool IsFullHealth => fill.IsMax;
    public bool IsEmpty => fill.IsEmpyt;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        fill.OnChanged -= FaintCheck;
        fill.OnChanged += FaintCheck;

        SetFillValue(Owner.Status);
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        fill.OnChanged -= FaintCheck;
    }
    protected void SetFillValue(UnitStatus unitstatus)
    {
        fill = new FillValue(unitstatus.maxHP, unitstatus.maxHP);
    }

    public int TakeDamage(in DamageStruct damageInfo)
    {
        fill.DecreaseCurrent(damageInfo.damageAmount);
        return damageInfo.damageAmount;
    }
    public int TakeRestore(in RestoreStruct restoreInfo)
    {
        fill.IncreaseCurrent(restoreInfo.restoreAmount);
        return restoreInfo.restoreAmount;
    }

    public void FaintCheck()
    {
        if(IsEmpty)
        {
            Owner.GetModule<AnimationModule>()?.AnimationByFaint();
            Owner.FaintNotify();
        }
    }
}
