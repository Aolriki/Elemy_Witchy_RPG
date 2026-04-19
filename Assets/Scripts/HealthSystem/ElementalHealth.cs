using UnityEngine;

public class ElementalHealth : Health
{
    [Header("Elemental Type")]
    public EElements enemyElement;

    [Header("Settings")]
    public bool arenaEnemy = false;

    private bool alreadyDied = false;

    protected override void Start()
    {
        base.Start();
        if (EnemyRespawnManager.instance != null)
            EnemyRespawnManager.instance.Register(this);
        else
            Debug.LogError("EnemyRespawnManager não encontrado na cena!", this);
    }

    public override void Death()
    {
        OnDeath?.Invoke();
        Drop();

        if (!alreadyDied && !arenaEnemy)
        {
            if (TryGetComponent<InsightReward>(out var insight))
                insight.GiveInsight();
        }

        alreadyDied = true;
        transform.parent.gameObject.SetActive(false);
    }

    public override void ResetLife()
    {
        base.ResetLife();
        StopAllCoroutines();
        invulnerable = false;
        // alreadyDied NÃO é resetado — inimigo não dá insight novamente após respawn
    }

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
        return rawDamage * multiplier;
    }
}