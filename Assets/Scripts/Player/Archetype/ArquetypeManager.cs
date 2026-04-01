using UnityEngine;
using System;

public class ArchetypeManager : MonoBehaviour
{
    public static ArchetypeManager Instance { get; private set; }
    public event Action onArchetypeSelected;

    [Header("Arquétipos Disponíveis")]
    [SerializeField] private ArchetypeData archetypeCautelosa;
    [SerializeField] private ArchetypeData archetypeExemplar;
    [SerializeField] private ArchetypeData archetypeProativa;

    public ArchetypeData CurrentArchetype { get; private set; }
    public bool HasArchetype => CurrentArchetype != null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SelectCautelosa() => SelectArchetype(archetypeCautelosa);
    public void SelectExemplar() => SelectArchetype(archetypeExemplar);
    public void SelectProativa() => SelectArchetype(archetypeProativa);

    private void SelectArchetype(ArchetypeData archetype)
    {
        if (archetype == null)
        {
            Debug.LogWarning("ArchetypeManager: arquétipo não atribuído no Inspector.");
            return;
        }

        CurrentArchetype = archetype;
        Debug.Log($"Arquétipo selecionado: {CurrentArchetype.archetypeName}");
        onArchetypeSelected?.Invoke();
    }

    public bool HasSelectedArchetype()
    {
        return HasArchetype;
    }

    public void ResetArchetype()
    {
        CurrentArchetype = null;
    }
}