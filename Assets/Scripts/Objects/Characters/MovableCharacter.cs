using UnityEngine;

public class MovableCharacter : CharacterBase
{
    [SerializeField] Animator anim;

    public void AnimationUpdate(Vector3 moveDelta)
    {
        if (!anim) return;

        anim.SetFloat("MoveX", LookRotation.x);
        anim.SetFloat("MoveY", LookRotation.y);
        anim.SetFloat("MoveSpeed", moveDelta.magnitude / Time.fixedDeltaTime);
    }
}