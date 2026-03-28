using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalSpawner : MonoBehaviour
{
    [Header("Objects")]
    [Tooltip("Lista de GameObjects desativados na cena que serão ativados ao disparar o trigger.")]
    public List<GameObject> objectsToSpawn = new List<GameObject>();

    [Header("Delay e Animator")]
    public Animator smokeAnimator;
    public float spawnDelay = 0f;

    private string smokeTriggerName = "PlaySmoke";
    private bool _hasActivated = false;

    // Metodo a ser chamado pelo Trigger
    public void Activate()
    {
        if (_hasActivated) return;
        _hasActivated = true;

        if (spawnDelay > 0f)
            StartCoroutine(SpawnWithDelay());
        else
            SpawnAll();
    }

    private void SpawnAll()
    {
        foreach (GameObject obj in objectsToSpawn)
        {
            if (obj == null) continue;
            PlaySmokeAt(obj.transform.position);
            obj.SetActive(true);
        }
    }

    // Delay entre cada objeto a ser spawnado
    private IEnumerator SpawnWithDelay()
    {
        foreach (GameObject obj in objectsToSpawn)
        {
            if (obj == null) continue;
            PlaySmokeAt(obj.transform.position);
            obj.SetActive(true);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Usa o Transform dos Objetos a serem spawnados para criar o efeito de fumaça
    private void PlaySmokeAt(Vector3 position)
    {
        if (smokeAnimator == null) return;

        smokeAnimator.transform.position = position;
        smokeAnimator.SetTrigger(smokeTriggerName);
    }


    // Opcional, caso eu queira reativar
    public void ResetSpawner()
    {
        _hasActivated = false;
    }
}
