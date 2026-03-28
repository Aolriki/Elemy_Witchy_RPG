using UnityEngine;

public enum EContextType
{
    None,
    Graphic,
    Physics,
    Combat
}

public abstract class Context : MonoBehaviour
{
    public abstract EContextType GetContextType();
}
