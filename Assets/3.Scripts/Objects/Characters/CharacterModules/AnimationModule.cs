using UnityEngine;

public class AnimationModule : CharacterModule
{
    [Header("Animation Settings")]
    [SerializeField] Animator anim;
    [SerializeField] bool isRotationByMovement;
    [SerializeField] CapsuleCollider mainCollider;
    [SerializeField] Rigidbody mainRigid;

    Rigidbody[] ragdollRigidbodies;
    public CapsuleCollider MainCollider => mainCollider;

    public sealed override System.Type RegistrationType => typeof(AnimationModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        newOwner.OnLookAt -= AnimationByLookRotation;
        newOwner.OnLookAt += AnimationByLookRotation;
        newOwner.OnMovement -= AnimationByMovement;
        newOwner.OnMovement += AnimationByMovement;
        newOwner.OnFaint -= AnimationByFaint;
        newOwner.OnFaint += AnimationByFaint;

        StageManager.OnStageStateChange -= StopAnimationByEndBattle;
        StageManager.OnStageStateChange += StopAnimationByEndBattle;

        //모든 rigid를 가져와 isKineatic을 true로 바꾼다
        GetAllRigidbody();
        SetRigidbodyAndCollier();
    }
    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        oldOwner.OnLookAt -= AnimationByLookRotation;
        oldOwner.OnMovement -= AnimationByMovement;
        oldOwner.OnFaint -= AnimationByFaint;
        StageManager.OnStageStateChange -= StopAnimationByEndBattle;
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

    //모든 하위 rigidbody 가져오기
    public void GetAllRigidbody()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
    }
    //rigidbody와 collider 세팅하기
    public void SetRigidbodyAndCollier()
    {
        if (ragdollRigidbodies == null || !mainCollider) return;

        //각 파츠의 rigidbody의 isKinematic을 true로 바꾸는 작업.
        foreach (Rigidbody rigid in ragdollRigidbodies) { rigid.isKinematic = true; }
        //메인 콜라이더가 꺼져있다면 켜주기
        mainCollider.enabled = true;
        mainRigid.isKinematic = true;
    }

    //hp가 0이 되면 실행해야 할 기능
    public void AnimationByFaint()
    {
        if (!anim) return;

        anim.enabled = false;
        mainCollider.enabled = false;
        foreach (Rigidbody rigid in ragdollRigidbodies) 
        { 
            rigid.isKinematic = false; 
        }
        mainRigid.isKinematic = true;
    }
    public void StopAnimationByEndBattle(StageState oldState, StageState newState)
    {
        if (!anim) return;

        if(newState == StageState.Result)
        {
            anim.speed = 0f;
        }
    }

    public void TriggerAnimation(string wantAnim)
    {
        anim.SetTrigger(wantAnim);
    }
   
}