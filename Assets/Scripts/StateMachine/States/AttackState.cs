using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class AttackState : State
{
    [Header("States")]
    public FloorState FloorState;
    public FallState FallState;

    public float attackDuration;
    public float hitPointTime;
    public Vector2 attackArea;
    public Vector2 hitAreaOffset;
    public LayerMask excludeHitLayerMask;
    public float damage;
    public float hitImpulse = 5f;

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

    private void Start()
    {
        if (PlayerStatus.Instance.IsReady)
            InitStats();
        else
            PlayerStatus.Instance.onStatsChanged += InitStats;
    }

    private void InitStats()
    {
        PlayerStatus.Instance.onStatsChanged -= InitStats;
        damage = PlayerStatus.Instance.Damage;
        PlayerStatus.Instance.onStatsChanged += UpdateDamageStats;
    }

    private void UpdateDamageStats()
    {
        damage = PlayerStatus.Instance.Damage;
    }


    public override void Enter()
    {
        StartCoroutine(HitCoroutine());
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        yield return new WaitForSeconds(hitPointTime);

        Vector2 direction = GraphicContext.IsFacingRight() ? Vector2.right : Vector2.left;

        Vector2 center = (Vector2)transform.position +
                         new Vector2(hitAreaOffset.x * direction.x, hitAreaOffset.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackArea, 0f, ~excludeHitLayerMask);

        if (hits.Length == 0)
            yield break;

        HashSet<IDamageable> damaged = new HashSet<IDamageable>();
        bool didHit = false;

        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null && !damaged.Contains(damageable))
            {
                damaged.Add(damageable);
                damageable.Damage(damage, core.transform.position, false);
                didHit = true;
            }
        }

        if (didHit)
        {
            Vector2 impulse = -direction * hitImpulse;
            PhysicsContext.movementVelocity.x = impulse.x;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        GraphicContext.animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDuration);

        if(PhysicsContext.grounded)
        {
            rootStateMachine.Set(FloorState);
        }
        else
            rootStateMachine.Set(FallState);
    }


    public override void InitializeSubState()
    {
    }

    protected override void Do()
    {
    }
    protected override void FixedDo()
    {
        if (rootStateMachine.previousState != FloorState)
            PhysicsContext.HandleGravity();

        PhysicsContext.CheckCollisions();

        if(PhysicsContext.grounded)
        {
            PhysicsContext.movementVelocity.y = 0;
            GraphicContext.animator.SetFloat("Speed Y", 0f);
        }
    }

    protected override void Exit()
    {
        StopAllCoroutines();
    }


    protected override void SelectState()
    {
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;

        Vector2 direction =  GraphicContext != null && GraphicContext.graphic.transform.localScale.x == 1
            ? Vector2.right
            : Vector2.left;
        
        Vector2 center = (Vector2)transform.position +
                         new Vector2(hitAreaOffset.x * direction.x, hitAreaOffset.y);

        Gizmos.DrawWireCube(center, attackArea);
    }

    private void UpdateStats()
    {
        damage = PlayerStatus.Instance.Damage;
    }

}
