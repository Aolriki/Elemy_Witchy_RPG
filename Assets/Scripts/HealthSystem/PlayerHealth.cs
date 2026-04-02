using UnityEngine;
using UnityEngine.Rendering;

public class PlayerHealth : Health
{
    [SerializeField] private Player player;

    protected override void Start()
    {
        if (PlayerStatus.Instance.IsReady)
            InitStats();
        else
            PlayerStatus.Instance.onStatsChanged += InitStats;
    }

    private void InitStats()
    {
        PlayerStatus.Instance.onStatsChanged -= InitStats;
        maxLife = PlayerStatus.Instance.MaxHealth;
        base.Start();
        PlayerStatus.Instance.onStatsChanged += UpdateHealthStats;
        UpdateUI();
    }

    private void UpdateHealthStats()
    {
        maxLife = PlayerStatus.Instance.MaxHealth;
        ResetLife();
    }

    public override void ResetLife()
    {
        base.ResetLife();
        UpdateUI();
    }

    public override void Damage(float damage, Vector3 damageDirection, bool heavyAttack)
    {
        if (invulnerable) return;
        Debug.Log("Tomei Dano");

        if (defending && !heavyAttack) return;

        //if(!string.IsNullOrEmpty(damageAudioName)) AudioManager.Instance.PlaySoundFXClip(transform.position, sfxName: damageAudioName, volume: 1);

        currentLife = Mathf.Clamp(currentLife - damage, 0, maxLife);

        if (currentLife <= 0)
        {
            UpdateUI(); // <-- linha adicionada
            Death();
            return;
        }

        if (invulnerabilityTime > 0) StartCoroutine(Invulnerable());


        if (doDamageEffect)
        {
            if (flashColors.Count > 0)
            {
                foreach (var color in flashColors)
                {
                    color.Flash(damageColor);
                }
            }
        }

        OnDamage?.Invoke(damageDirection);

        player.CombatContext.damageDirection = damageDirection;
        player.Set(player.damageState);

        UpdateUI();
    }

    public override void Heal(float heal)
    {
        base.Heal(heal);
        UpdateUI();
    }

    public override void Death()
    {
        base.Death();

        player.Set(player.deathState);
        player.PhysicsContext.movementVelocity = Vector3.zero;
        player.PhysicsContext.rb.linearVelocity = Vector3.zero;
        StartCoroutine(LevelManager.instance.PlayerRespawn());

        EnemyRespawnManager.instance.OnPlayerDeath();
    }

    protected void UpdateUI()
    {
        ScreenManager.Instance.UpdateHealth((int)currentLife, (int)maxLife);
    }

    private void UpdateStats()
    {
        maxLife = PlayerStatus.Instance.MaxHealth;
        ResetLife();
    }
}
