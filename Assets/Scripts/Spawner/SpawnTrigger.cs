using UnityEngine;
using UnityEngine.Events;

public class SpawnTrigger : MonoBehaviour
{
    [Header("Identificação")]
    public string triggerID;

    [Header("Spawner Alvo")]
    public ConditionalSpawner targetSpawner;

    [Header("Ao Ativar")]
    public UnityEvent onTriggerActivated;

    private bool _triggered = false;

    private void Awake()
    {
        if (targetSpawner == null)
            Debug.LogError($"[SpawnTrigger] '{gameObject.name}' (ID: '{triggerID}') não tem um targetSpawner atribuído!", this);

        if (string.IsNullOrEmpty(triggerID))
            Debug.LogWarning($"[SpawnTrigger] '{gameObject.name}' está sem triggerID. Risco de conflito!", this);
    }

    public void Activate()
    {
        if (_triggered)
        {
            Debug.LogWarning($"[SpawnTrigger] '{triggerID}' já foi ativado. Chamada ignorada.");
            return;
        }

        if (targetSpawner == null)
        {
            Debug.LogError($"[SpawnTrigger] '{triggerID}' não pode ativar: targetSpawner está nulo!");
            return;
        }

        _triggered = true;
        targetSpawner.Activate();
        onTriggerActivated?.Invoke();

        Debug.Log($"[SpawnTrigger] '{triggerID}' ativado com sucesso.");
    }

    public void ResetTrigger()
    {
        _triggered = false;
    }

    public bool IsTriggered => _triggered;
    public string TriggerID => triggerID;
}
