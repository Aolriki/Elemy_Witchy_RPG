using UnityEngine;

public class JumpState : State
{
    [Header("States")]
    public FallState fallState;
    public AttackState attackState;

    [Header("Contexts")]
    private GraphicContext GraphicContext;
    private PhysicsContext PhysicsContext;
    private CombatContext AttackContext;

    public override void Init()
    {
        if (core.contextDict.TryGetValue(EContextType.Graphic, out var graphicContext))
        {
            GraphicContext = (GraphicContext)graphicContext;
        }
        else
        {
            Debug.LogError("LocomotionState: Missing GraphicContext in core context dictionary.");
            enabled = false;
        }

        if (core.contextDict.TryGetValue(EContextType.Physics, out var physicsContext))
        {
            PhysicsContext = (PhysicsContext)physicsContext;
        }
        else
        {
            Debug.LogError("LocomotionState: Missing PhysicsContext in core context dictionary.");
            enabled = false;
        }

        if (core.contextDict.TryGetValue(EContextType.Combat, out var attackContext))
        {
            AttackContext = (CombatContext)attackContext;
        }
        else
        {
            Debug.LogError("Missing AttackContext in core context dictionary.");
            enabled = false;
        }
    }

    public override void Enter()
    {
        GraphicContext.animator.SetBool("Grounded", false);
        PhysicsContext.movementVelocity.y = PhysicsContext.jumpPower;
    }

    public override void InitializeSubState()
    {
    }

    protected override void Do()
    {
        GraphicContext.animator.SetFloat("Speed Y", PhysicsContext.rb.linearVelocityY);

        SelectState();
    }


    protected override void FixedDo()
    {
        PhysicsContext.HandleGravity();
        PhysicsContext.HandleMovement(GraphicContext);
        PhysicsContext.CheckCollisions();
    }
    protected override void SelectState()
    {
        if(PhysicsContext.orderingJump == false || PhysicsContext.movementVelocity.y <= 0)
        {
            PhysicsContext.movementVelocity.y = Mathf.Min(0, PhysicsContext.movementVelocity.y); ;
            core.rootStateMachine.Set(fallState);
        }

        if(PhysicsContext.hitCeiling)
        {
            PhysicsContext.movementVelocity.y = Mathf.Min(0, PhysicsContext.movementVelocity.y);
            core.rootStateMachine.Set(fallState);
        }

        if (AttackContext.onAttack)
            rootStateMachine.Set(attackState);
    }

    protected override void Exit()
    {
    }
}
