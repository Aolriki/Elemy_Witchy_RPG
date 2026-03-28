using System.Collections;
using UnityEngine;

public class TakeDamageState : State
{
    [Header("States")]
    public FallState fallState;
    public FloorState floorState;

    [Header("Contexts")]
    private GraphicContext GraphicContext;
    private PhysicsContext PhysicsContext;
    private CombatContext CombatContext;

    public float damageImpulse = 5f;
    public float blockMovementDuration = 0.3f;

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
            CombatContext = (CombatContext)attackContext;
        }
        else
        {
            Debug.LogError("Missing AttackContext in core context dictionary.");
            enabled = false;
        }
    }

    public override void Enter()
    {
        Vector3 knockbackDir = (core.transform.position - CombatContext.damageDirection).normalized;
        PhysicsContext.movementVelocity = knockbackDir * damageImpulse;

        StartCoroutine(BlockMovementCoroutine());
    }

    private IEnumerator BlockMovementCoroutine()
    {
        GraphicContext.animator.SetTrigger("Damage");

        yield return new WaitForSeconds(blockMovementDuration);

        if (PhysicsContext.grounded)
        {
            rootStateMachine.Set(floorState);
        }
        else
            rootStateMachine.Set(fallState);
    }

    public override void InitializeSubState()
    {
    }

    protected override void Do()
    {
    }

    protected override void Exit()
    {
    }

    protected override void FixedDo()
    {
        if (rootStateMachine.previousState != floorState)
            PhysicsContext.HandleGravity();

        PhysicsContext.CheckCollisions();

        if (PhysicsContext.grounded)
        {
            PhysicsContext.movementVelocity.y = 0;
        }
    }

    protected override void SelectState()
    {
    }
}
