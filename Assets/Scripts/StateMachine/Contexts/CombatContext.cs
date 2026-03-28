using UnityEngine;

public class CombatContext : Context
{
    public override EContextType GetContextType()
    {
        return EContextType.Combat;
    }

    public bool onAttack;

    [HideInInspector] public EElements currentElement; // Elemento do ataque atual

    [HideInInspector] public Vector3 damageDirection;
}
