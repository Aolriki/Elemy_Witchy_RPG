using UnityEngine;

[CreateAssetMenu(fileName = "ArchetypeData", menuName = "Player/Archetype")]
public class ArchetypeData : ScriptableObject
{
    [Header("Identidade")]
    public string archetypeName;
    [TextArea(3, 6)]
    public string description;
    public Sprite illustration;

    [Header("Atributos Base")]
    public float elemyTimer = 6f;
    public int damage = 3;
    public float maxHealth = 4f;
}