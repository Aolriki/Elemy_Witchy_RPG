using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class FloorState : State
{
    [Header("States")]
    public IdleState idleState;
    public MoveState moveState;
    public FallState fallState;
    public JumpState jumpState;
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
            Debug.LogError("Missing GraphicContext in core context dictionary.");
            enabled = false;
        }

        if (core.contextDict.TryGetValue(EContextType.Physics, out var physicsContext))
        {
            PhysicsContext = (PhysicsContext)physicsContext;
        }
        else
        {
            Debug.LogError("Missing PhysicsContext in core context dictionary.");
            enabled = false;
        }

        if (core.contextDict.TryGetValue(EContextType.Combat, out var attackContext))
        {
            AttackContext = (CombatContext)attackContext;
        }
        else
        {
            Debug.LogError("Missing GraphicContext in core context dictionary.");
            enabled = false;
        }
    }

    public override void Enter()
    {
        PhysicsContext.timeEndedFall = Time.time;

        PhysicsContext.movementVelocity.y = 0;
        GraphicContext.animator.SetFloat("Speed Y", 0f);

        if (PhysicsContext.HasJumpBuffer())
            rootStateMachine.Set(jumpState);

        GraphicContext.animator.SetBool("Grounded", true);
    }

    public override void InitializeSubState()
    {
    }

    protected override void Do()
    {
        SelectState();
    }
    protected override void FixedDo()
    {
        PhysicsContext.HandleMovement(GraphicContext);
        PhysicsContext.CheckCollisions();
    }
    protected override void SelectState()
    {
        float moveXVelocity = PhysicsContext.movementVelocity.x;

        if (Mathf.Abs(moveXVelocity) < 0.01f)
            machine.Set(idleState);
        else
            machine.Set(moveState);

        if (!PhysicsContext.grounded)
            rootStateMachine.Set(fallState);
        else if (PhysicsContext.onJump)
            rootStateMachine.Set(jumpState);

        if(AttackContext.onAttack)
            rootStateMachine.Set(attackState);
    }

    protected override void Exit()
    {
    }




}
