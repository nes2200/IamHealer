using UnityEngine;

public class HostileAIController : AIController 
{
    protected override void OnPossess(CharacterBase newCharacter)
    {
        GameManager.OnUpdateController -= Think;
        GameManager.OnUpdateController += Think;
    }
    protected override void OnUnpossess(CharacterBase oldCharacter)
    {
        GameManager.OnUpdateController -= Think;
    }

    protected override void Think(float deltaTime)
    {
        if (!FocusTarget) return;
        CommandMoveToDestination(FocusTarget.transform.position, 0.5f);
    }

    public void Attack()
    {
        AttackModule atkModule = Character.GetModule<AttackModule>();
        atkModule.AttackTarget(new AttackInfo
        {
            target = FocusTarget,
            instigator = this,
            damageAmount = Character.Status.damage
        });
    }
}