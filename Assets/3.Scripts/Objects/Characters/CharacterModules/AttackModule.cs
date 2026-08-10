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
    bool isAttackCooldown = false;
    public bool IsAttackCooldown => isAttackCooldown;
    float attackCooldownCurrent;

    public override Type RegistrationType => typeof(AttackModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
    }

    public void AttackTarget(in AttackInfo attackInfo)
    {
        if (isAttackCooldown)
        {
            return;
        }

        HitPointModule targetHPModule = attackInfo.target.GetComponent<HitPointModule>();
        if (!targetHPModule) return;
        targetHPModule.TakeDamage(new DamageStruct
        {
            from = Owner.gameObject,
            instigator = attackInfo.instigator,
            damageAmount = attackInfo.damageAmount
        });
        AnimationModule animModule = Owner.GetModule<AnimationModule>();
        animModule.TriggerAnimation("Attack");
        AttackCooldownStart();
    }

    public void AttackCooldownStart()
    {
        GameManager.OnUpdateCharacter -= AttackCooldownUpdate;
        GameManager.OnUpdateCharacter += AttackCooldownUpdate;
        isAttackCooldown = true;
    }
    public void AttackCooldownEnd()
    {
        GameManager.OnUpdateCharacter -= AttackCooldownUpdate;
        attackCooldownCurrent = 0f;
        isAttackCooldown = false;
    }

    void AttackCooldownUpdate(float deltaTime)
    {
        attackCooldownCurrent += deltaTime;
        if(attackCooldownCurrent >= Owner.Status.attackSpeed)
        {
            AttackCooldownEnd();
        }
    }
}