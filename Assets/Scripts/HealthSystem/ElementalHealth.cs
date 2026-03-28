using UnityEngine;

public class ElementalHealth : Health
{
    [Header("Elemental Type")]
    public EElements enemyElement;

    public override void Damage(float damage, Vector3 damageDirection, bool heavyAttack)
    {
        float modifiedDamage = ApplyElementalModifier(damage);
        base.Damage(modifiedDamage, damageDirection, heavyAttack);
    }

    private float ApplyElementalModifier(float rawDamage)
    {
        EElements attackerElement = Player.instance.currentElement;

        float multiplier = enemyElement switch
        {
            EElements.Igna => attackerElement switch
            {
                EElements.Aqua => 3.0f,
                EElements.Flora => 0.5f,
                EElements.None => 1.0f,
                EElements.Igna => 0.1f,
                _ => 1.0f
            },
            EElements.Flora => attackerElement switch
            {
                EElements.Igna => 3.0f,
                EElements.Aqua => 0.5f,
                EElements.None => 1.0f,
                EElements.Flora => 0.1f,
                _ => 1.0f
            },
            EElements.Aqua => attackerElement switch
            {
                EElements.Flora => 3.0f,
                EElements.Igna => 0.5f,
                EElements.None => 1.0f,
                EElements.Aqua => 0.1f,
                _ => 1.0f
            },
            EElements.None => attackerElement switch
            {
                _ => 1.0f
            },
            _ => 1.0f
        };

        Debug.Log($"Enemy: {enemyElement} | Attacker: {attackerElement} | x{multiplier} | {rawDamage} -> {rawDamage * multiplier}");
        return rawDamage * multiplier;
    }
}