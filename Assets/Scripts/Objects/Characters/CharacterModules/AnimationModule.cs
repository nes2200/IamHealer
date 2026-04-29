using UnityEngine;

public class AnimationModule : CharacterModule
{
    [SerializeField] Animator anim;
    [SerializeField] bool isRotationByMovement;

    public sealed override System.Type RegistrationType => typeof(AnimationModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        newOwner.OnLookAt -= AnimationByLookRotation;
        newOwner.OnLookAt += AnimationByLookRotation;
        newOwner.OnMovement -= AnimationByMovement;
        newOwner.OnMovement += AnimationByMovement;
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        oldOwner.OnLookAt -= AnimationByLookRotation;
        oldOwner.OnMovement -= AnimationByMovement;
    }

    public void AnimationByLookRotation(Vector3 lookRotation)
    {
        if (!anim) return;
        //                            world로 들어온 벡터를 local로 돌린다
        Vector3 localRotation = transform.InverseTransformVector(lookRotation).normalized;
        anim.SetFloat("MoveX", localRotation.x);
        anim.SetFloat("MoveZ", localRotation.z);
    }
    public void AnimationByMovement(Vector3 moveDelta)
    {
        if (!anim) return;
        if (isRotationByMovement && moveDelta.sqrMagnitude > 0)
        {
            AnimationByLookRotation(moveDelta);
        }
        anim.SetFloat("MoveSpeed", moveDelta.magnitude / Time.fixedDeltaTime);
    }
    public void AnimationByRotation(Vector3 rotation)
    {

    }
}