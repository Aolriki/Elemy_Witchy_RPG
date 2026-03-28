using UnityEngine;

public interface IDamageable
{
    void Damage(float damage, Vector3 damageDirection, bool heavyAttack = false);

    void Heal(float heal);
}
