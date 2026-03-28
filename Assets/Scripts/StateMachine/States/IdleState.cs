using UnityEngine;

public class IdleState : State
{
    private GraphicContext GraphicContext;
    private PhysicsContext PhysicsContext;

    public override void Init()
    {
        if (core.contextDict.TryGetValue(EContextType.Graphic, out var graphicContext))
        {
            GraphicContext = (GraphicContext)graphicContext;
        }
        else
        {
            Debug.LogError("IdleState: Missing GraphicContext in core context dictionary.");
            enabled = false;
        }

        if(core.contextDict.TryGetValue(EContextType.Physics, out var physicsContext))
        {
            PhysicsContext = (PhysicsContext)physicsContext;
        }
        else
        {
            Debug.LogError("IdleState: Missing PhysicsContext in core context dictionary.");
            enabled = false;
        }
    }

    public override void Enter()
    {
    }

    public override void InitializeSubState()
    {
    }

    protected override void Do()
    {
        GraphicContext.animator.SetFloat("Speed X", Mathf.Abs(PhysicsContext.rb.linearVelocity.x));
    }

    protected override void Exit()
    {
    }

    protected override void FixedDo()
    {
    }

    protected override void SelectState()
    {
    }
}
