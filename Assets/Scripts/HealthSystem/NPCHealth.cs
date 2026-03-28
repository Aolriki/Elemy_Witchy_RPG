using UnityEngine;

public class NPCHealth : Health
{
    public HealthSliderUpdater healthSliderUpdater;

    public override void ResetLife()
    {
        base.ResetLife();
        UpdateUI();
    }

    public override void Damage(float damage, Vector3 damageDirection, bool heavyAttack)
    {
        base.Damage(damage, damageDirection, heavyAttack);
        UpdateUI();
    }

    protected void UpdateUI()
    {
        if (healthSliderUpdater != null)
        {
            if (!healthSliderUpdater.gameObject.activeInHierarchy && currentLife != maxLife && currentLife != 0) healthSliderUpdater.gameObject.SetActive(true);
            healthSliderUpdater.UpdateHealthUI(currentLife / maxLife);
        }
    }
}
