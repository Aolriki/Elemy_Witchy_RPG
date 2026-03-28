using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysicsContext : Context
{
    public override EContextType GetContextType()
    {
        return EContextType.Physics;
    }

    public Rigidbody2D rb;
    public Collider2D col;
    public LayerMask groundLayer;
    public float extraDistanceToCheckCollisions = 0.05f;

    [Header("Movement")]
    public Vector2 movementDirection;
    public Vector2 movementVelocity;

    //Ground
    public float maxGroundSpeed = 12f;
    public float GroundAcceleration = 60f;
    public float GroundDecel = 120f;
    public bool grounded = false;

    //Air
    public float maxAirSpeed = 6f;
    public float AirAcceleration = 30f;
    public float AirDecel = 30f;

    //Fall
    public float fallGravityMultiplier = 1.0f;
    public float gravityAcceleration = 110f;
    public float maxFallSpeed = 25f;

    //Jump
    [HideInInspector] public bool onJump = false;
    [HideInInspector] public bool orderingJump = false;
    public float jumpPower = 36f;
    public bool hitCeiling = false;
    public float jumpRisingDivider = 2f;

    // Double Jump
    public bool doubleJumpUnlocked = false;   // ativada pelo item coletado
    [HideInInspector] public bool hasDoubleJump = false;  // consumida ao usar

    //Coyote
    public float coyoteTime = 0.2f;
    [HideInInspector] public float timeStartedToFall;

    //JumpBuffer
    public float jumpBufferTime = 0.2f;
    [HideInInspector] public float timeTriedToJumpBuffer;
    [HideInInspector] public float timeEndedFall;



    private void FixedUpdate()
    {
        ApplyMovementVelocity();
    }

    private void ApplyMovementVelocity() => rb.linearVelocity = movementVelocity;


    #region Helpers

    public void HandleMovement(GraphicContext graphicContext = null)
    {
        float moveXDirection = movementDirection.x;

        if (Mathf.Abs(moveXDirection) > 0.01f)
        {
            var maxSpeed = grounded ? maxGroundSpeed : maxAirSpeed;
            var acceleration = grounded ? GroundAcceleration : AirAcceleration;
            movementVelocity.x = Mathf.MoveTowards(movementVelocity.x, moveXDirection * maxSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            var deceleration = grounded ? GroundDecel : AirDecel;
            movementVelocity.x = Mathf.MoveTowards(movementVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }

        if (graphicContext)
            FaceDirection(graphicContext);
    }

    public void FaceDirection(GraphicContext graphicContext)
    {
        if (Mathf.Abs(movementDirection.x) > 0.01f)
        {
            float xScale = (movementDirection.x < 0) ? -1 : 1f;
            graphicContext.graphic.transform.localScale = new Vector3(xScale, 1, 1);
        }
    }

    public void HandleGravity()
    {
        if (!grounded)
        {
            //if we're falling, we want to apply more gravity than if we're rising, this makes the jump feel better and more responsive
            var inAirGravity = movementVelocity.y < 0 ? gravityAcceleration * fallGravityMultiplier : gravityAcceleration / jumpRisingDivider;
            movementVelocity.y = Mathf.MoveTowards(movementVelocity.y, -maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    public void CheckCollisions()
    {
        Bounds bounds = col.bounds;
        Vector2 size = new Vector2(bounds.size.x * 0.9f, 0.02f);

        // ===== GROUND CHECK =====
        Vector2 groundOrigin = new Vector2(bounds.center.x, bounds.min.y);

        RaycastHit2D groundHit = Physics2D.BoxCast(
            groundOrigin,
            size,
            0f,
            Vector2.down,
            extraDistanceToCheckCollisions,
            groundLayer
        );

        bool validGroundHit = groundHit.collider != null && !groundHit.collider.isTrigger;

        // ===== CEILING CHECK =====
        Vector2 ceilingOrigin = new Vector2(bounds.center.x, bounds.max.y);

        RaycastHit2D ceilingHit = Physics2D.BoxCast(
            ceilingOrigin,
            size,
            0f,
            Vector2.up,
            extraDistanceToCheckCollisions,
            groundLayer
        );

        bool validCeilingHit = ceilingHit.collider != null && !ceilingHit.collider.isTrigger;


        hitCeiling = validCeilingHit;            

        grounded = validGroundHit;

        // Ao tocar o chão, restaura o pulo duplo
        if (grounded)
            hasDoubleJump = doubleJumpUnlocked;
    }

    // Arthur Aqui. Metodo para o DoubleJump
    public bool TryDoubleJump()
    {
        if (!doubleJumpUnlocked) return false;
        if (grounded) return false;
        if (!hasDoubleJump) return false;

        movementVelocity.y = jumpPower;
        hasDoubleJump = false;
        return true;
    }

    public bool CanCoyoteTime()
    {
        return Time.time > timeStartedToFall && (Time.time - timeStartedToFall) < coyoteTime;
    }

    public bool HasJumpBuffer()
    {
        return timeEndedFall > timeTriedToJumpBuffer && (timeEndedFall - timeTriedToJumpBuffer) < jumpBufferTime;
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (col == null) return;

        Bounds bounds = col.bounds;

        Vector2 size = new Vector2(bounds.size.x * 0.9f, 0.02f);
        Vector2 groundOrigin = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 ceilingOrigin = new Vector2(bounds.center.x, bounds.max.y);

        // =============================
        // GROUND CHECK GIZMO
        // =============================
        Gizmos.color = Color.green;

        Vector3 groundCastEnd = groundOrigin + Vector2.down * extraDistanceToCheckCollisions;

        // Box inicial
        Gizmos.DrawWireCube(groundOrigin, size);

        // Box final do cast
        Gizmos.DrawWireCube(groundCastEnd, size);

        // Linha conectando
        Gizmos.DrawLine(groundOrigin, groundCastEnd);

        // =============================
        // CEILING CHECK GIZMO
        // =============================
        Gizmos.color = Color.red;

        Vector3 ceilingCastEnd = ceilingOrigin + Vector2.up * extraDistanceToCheckCollisions;

        Gizmos.DrawWireCube(ceilingOrigin, size);
        Gizmos.DrawWireCube(ceilingCastEnd, size);
        Gizmos.DrawLine(ceilingOrigin, ceilingCastEnd);
    }

}
