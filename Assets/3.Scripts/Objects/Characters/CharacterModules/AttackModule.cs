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
    //공격 쿨다운
    bool isAttackCooldown = false;
    public bool IsAttackCooldown => isAttackCooldown;
    float attackCooldownCurrent;

    //공격 진행
    float hitNormalizedTime;
    bool hasAppliedAttack = false;
    AttackInfo currentAttackInfo;
    bool isAttacking = false;
    public bool IsAttacking => isAttacking;


    //애니메이션 모듈
    AnimationModule animModule;

    public override Type RegistrationType => typeof(AttackModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        animModule = Owner.GetModule<AnimationModule>();
        hitNormalizedTime = Owner.Status.hitNormalizedTime;
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
    }

    public void AttackTarget(in AttackInfo attackInfo)
    {
        if (isAttackCooldown || isAttacking) return;

        isAttacking = true;
        animModule.SetBool("IsAttacking", true);
        animModule.TriggerAnimation("Attack");

        currentAttackInfo = attackInfo;
        hasAppliedAttack = false;

        GameManager.OnUpdateCharacter -= UpdateAttack;
        GameManager.OnUpdateCharacter += UpdateAttack;
    }

    void UpdateAttack(float deltaTime)
    {
        if (!isAttacking || hasAppliedAttack) return;

        if (!animModule.TryGetNormalizedTime(out float normalizedProgress)) return;

        if (normalizedProgress < hitNormalizedTime) return;
        
        hasAppliedAttack = true;
        GameManager.OnUpdateCharacter -= UpdateAttack;

        ApplyAttack(currentAttackInfo);
    }

    void ApplyAttack(AttackInfo attackInfo)
    {
        //적 체력 감소
        HitPointModule targetHPModule = attackInfo.target.GetComponent<HitPointModule>();
        if (!targetHPModule) return;
        targetHPModule.TakeDamage(new DamageStruct
        {
            from = Owner.gameObject,
            instigator = attackInfo.instigator,
            damageAmount = attackInfo.damageAmount
        });
    }

    public void OnAttackAnimationEnd()
    {
        if (!isAttacking) return;

        isAttacking = false;
        animModule.SetBool("IsAttacking", false);

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