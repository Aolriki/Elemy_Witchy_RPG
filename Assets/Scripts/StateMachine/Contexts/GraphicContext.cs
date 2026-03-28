using UnityEngine;

public class GraphicContext : Context
{
    public override EContextType GetContextType()
    {
        return EContextType.Graphic;
    }

    public GameObject graphic;
    public Animator animator;

    public bool IsFacingRight()
    {
        return graphic.transform.localScale.x == 1f;
    }
}
