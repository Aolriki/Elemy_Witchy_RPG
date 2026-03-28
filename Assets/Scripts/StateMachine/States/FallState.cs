using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FallState : State
{
    [Header("States")]
    public FloorState floorState;
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
            Debug.LogError("Missing GraphicContext in core context dictionary.");
            enabled = false;
        }
    }

    public override void Enter()
    {
        GraphicContext.animator.SetBool("Grounded", false);

        if(parent.previousState == floorState)
        {
            PhysicsContext.timeStartedToFall = Time.time;
        }
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
        PhysicsContext.HandleMovement(GraphicContext);
        PhysicsContext.HandleGravity();
        PhysicsContext.CheckCollisions();
    }
    protected override void SelectState()
    {
        if (PhysicsContext.grounded)
        {
            core.rootStateMachine.Set(floorState);
        }
        else if (PhysicsContext.onJump)
        {
            PhysicsContext.timeTriedToJumpBuffer = Time.time;

            if (PhysicsContext.CanCoyoteTime())
            {
                rootStateMachine.Set(jumpState);
            }
            else if (PhysicsContext.TryDoubleJump()) // só chega aqui se coyote falhou
            {
                rootStateMachine.Set(jumpState);
            }
        }
        if (AttackContext.onAttack)
            rootStateMachine.Set(attackState);
    }

    protected override void Exit()
    {
    }

}
