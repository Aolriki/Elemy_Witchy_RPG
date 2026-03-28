using UnityEngine;

public class ElementalWall : Health
{
    [Header("Elemental Type")]
    public EElements wallElement;

    public override void Damage(float damage, Vector3 damageDirection, bool heavyAttack)
    {
        if (!IsVulnerableTo(Player.instance.currentElement))
        {
            Debug.Log($"Wall [{wallElement}] is immune to {Player.instance.currentElement}");
            return;
        }

        // Passa Vector3.zero para ignorar knockback na classe base
        base.Damage(damage, Vector3.zero, heavyAttack);
    }

    private bool IsVulnerableTo(EElements attackerElement)
    {
        return wallElement switch
        {
            EElements.Flora => attackerElement == EElements.Igna,
            EElements.Aqua => attackerElement == EElements.Flora,
            EElements.Igna => attackerElement == EElements.Aqua,
            _ => false
        };
    }
}