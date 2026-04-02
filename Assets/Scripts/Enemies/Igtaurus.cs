using UnityEngine;

public class Igtaurus : EnemyClass
{
    [Header("Detection — Raycast")]
    public float detectionRange = 6f;
    [Tooltip("Offset vertical a partir do pivot (base) para a origem do raycast.")]
    public float detectionHeightOffset = 0.5f;
    public LayerMask playerLayer;

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashSpeed = 12f;
    public float windUpTime = 0.8f;
    public float maxDashTime = 0.5f;

    [Header("Detection VFX")]
    public GameObject detectionIcon;

    private enum State { Patrolling, WindUp, Dashing, CoolDown }
    private State currentState = State.Patrolling;

    private float windUpTimer = 0f;
    private float dashTimer = 0f;
    private Vector2 dashTarget;
    private float facingDirection = 1f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                base.Update();
                DetectPlayer();
                break;

            case State.WindUp:
                HandleWindUp();
                break;

            case State.Dashing:
                HandleDash();
                break;

            case State.CoolDown:
                HandleCoolDown();
                break;
        }
    }

    #region Detecção
    private void DetectPlayer()
    {
        // Só detecta se o jogador estiver dentro da zona de patrulha
        Vector2 origin = (Vector2)transform.position + Vector2.up * detectionHeightOffset;
        Vector2 direction = Vector2.right * facingDirection;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, detectionRange, playerLayer);

        if (hit.collider == null) return;

        // Jogador precisa estar dentro da zona para disparar o ataque
        if (!IsInsidePatrolZone(hit.transform.position.x)) return;

        Debug.Log("Igtaurus: jogador avistado — iniciando wind up!");
        currentState = State.WindUp;
        windUpTimer = windUpTime;

        if (detectionIcon != null) detectionIcon.SetActive(true);
    }
    #endregion

    #region Wind Up
    private void HandleWindUp()
    {
        windUpTimer -= Time.deltaTime;

        if (windUpTimer <= 0f)
        {
            dashTarget = (Vector2)transform.position + new Vector2(facingDirection * dashDistance, 0f);
            dashTimer = 0f;
            currentState = State.Dashing;

            if (detectionIcon != null) detectionIcon.SetActive(false);
        }
    }
    #endregion

    #region Dash
    private void HandleDash()
    {
        dashTimer += Time.deltaTime;

        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            dashTarget,
            dashSpeed * Time.deltaTime
        );
        rb.MovePosition(newPosition);

        bool reachedTarget = Vector2.Distance(rb.position, dashTarget) < 0.1f;
        bool timedOut = dashTimer >= maxDashTime;

        if (reachedTarget || timedOut)
        {
            dashTimer = 0f;
            currentState = State.CoolDown;
            windUpTimer = windUpTime;
        }
    }
    #endregion

    #region CoolDown
    private void HandleCoolDown()
    {
        windUpTimer -= Time.deltaTime;

        if (windUpTimer <= 0f)
        {
            Debug.Log("Igtaurus: cooldown finalizado — voltando a patrulhar.");
            currentState = State.Patrolling;
            StartPatrol();
        }
    }
    #endregion

    protected override void SetFacingDirection(float directionX)
    {
        if (directionX == 0) return;
        facingDirection = Mathf.Sign(directionX);
        base.SetFacingDirection(directionX);
    }

    #region Gizmos
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Amarelo: detecção — respeita o offset vertical
        Vector3 rayOrigin = transform.position + Vector3.up * detectionHeightOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rayOrigin, rayOrigin + new Vector3(facingDirection * detectionRange, 0f, 0f));

        // Vermelho: distância do dash — parte do pivot (chão)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(facingDirection * dashDistance, 0f, 0f));
    }
    #endregion
}