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
    protected FillValue fill = new FillValue(10, 20);
    public override Type RegistrationType => typeof(HitPointModule);

    public int Max => fill.Max;
    public int Min => fill.Min;
    public bool IsFullHealth => fill.IsMax;
    public bool IsEmpty => fill.IsEmpyt;

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
   
}
