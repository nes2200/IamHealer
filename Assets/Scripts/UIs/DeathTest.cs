using UnityEngine;

public class DeathTest : MonoBehaviour
{
   [SerializeField] HitPointModule hpModule;

    public void Death()
    {
        hpModule.TakeDamage(new DamageStruct { damageAmount = hpModule.Max });
    }
}
