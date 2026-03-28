using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("Structure")]
    [SerializeField] private GameObject healthPointPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private HealthPoint[] healthPointsArray;

    [Header("Data")]
    [Range(1, 10)]
    [SerializeField] public int _maxHealth = 3;
    [SerializeField] private int _currentHealth = 2;

    public void UpdateHealth (int newCurrentHealth, int newMaxHealth)
    {
        if (_maxHealth != newMaxHealth)
        {
            _currentHealth = newCurrentHealth;
            UpdateMaxHealth(newMaxHealth);
        }
        else if (_currentHealth != newCurrentHealth)
        {
            UpdateCurrentHealth(newCurrentHealth);
        }
    }
    public void UpdateMaxHealth(int newMaxHealth)
    {
        _maxHealth = newMaxHealth;
        AdjustHitpoints();
        UpdateCurrentHealth(_currentHealth);
    }
    private void AdjustHitpoints()
    {
        int currentAmount = healthPointsArray.Length;

        if (_maxHealth > currentAmount)
        {
            int amountToCreate = _maxHealth - currentAmount;

            System.Array.Resize(ref healthPointsArray, _maxHealth);

            for (int i = currentAmount; i < _maxHealth; i++)
            {
                GameObject obj = Instantiate(healthPointPrefab, container);
                healthPointsArray[i] = obj.GetComponent<HealthPoint>();
            }
        }
        else if (_maxHealth < currentAmount)
        {
            for (int i = _maxHealth; i < currentAmount; i++)
            {
                Destroy(healthPointsArray[i].gameObject);
            }

            System.Array.Resize(ref healthPointsArray, _maxHealth);
        }
    }
    public void UpdateCurrentHealth(int newCurrentHealth)
    {
        newCurrentHealth = Mathf.Clamp(newCurrentHealth, 0, _maxHealth);

        _currentHealth = newCurrentHealth;
        ApplyHealthToPoints();


    }
    private int GetCurrentDisplayedHealth()
    {
        int currentDisplayedHealth = 0;
        foreach (HealthPoint healthPoint in healthPointsArray)
        {
            if (healthPoint._hasHealth == true)
            {
                currentDisplayedHealth++;
            }
        }
        return currentDisplayedHealth;
    }
    private void ApplyHealthToPoints()
    {
        for (int i = 0; i < healthPointsArray.Length; i++)
        {
            bool isActive = (i < _currentHealth);
            healthPointsArray[i].SetHealth(isActive);
        }
    }
}
