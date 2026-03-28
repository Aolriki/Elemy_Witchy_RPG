using UnityEngine;

public class Igtaurus : EnemyClass
{
    [Header("Detection")]
    public float detectionRange = 6f;
    

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashSpeed = 12f;
    public float windUpTime = 0.8f;     // Tempo parado antes do dash
    private float dashTimer = 0f;
    public float maxDashTime = 0.5f; // tempo máximo do dash

    [Header("Detection VFX")]
    public GameObject detectionIcon;

    // Estado interno
    private enum State { Patrolling, WindUp, Dashing, CoolDown }
    private State currentState = State.Patrolling;

    private float windUpTimer = 0f;
    private Vector2 dashTarget;
    private float facingDirection = 1f; // 1 = direita, -1 = esquerda
    private LayerMask playerLayer;

    protected override void Start()
    {
        base.Start();
        playerLayer = LayerMask.GetMask("Player"); // Busca a layer "Player" automaticamente
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
        // Teste SEM filtro de layer — se funcionar, o problema é na playerLayer
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (hit == null) return;

        // Checa se o jogador está na frente com base no facing direction
        float directionToPlayer = hit.transform.position.x - transform.position.x;
        bool playerIsInFront = Mathf.Sign(directionToPlayer) == Mathf.Sign(facingDirection);

        if (!playerIsInFront) return;

        // Jogador avistado — inicia wind up
        Debug.Log("Player spotted!");
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
            // Define o alvo do dash com base na direção que está olhando
            dashTarget = (Vector2)transform.position + new Vector2(facingDirection * dashDistance, 0f);
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
    #region Cooldown
    private void HandleCoolDown()
    {
        windUpTimer -= Time.deltaTime;
        if (windUpTimer <= 0f)
        {
            Debug.Log("Cooldown finished — returning to patrol.");
            currentState = State.Patrolling;
            StartPatrol();
        }
    }
    #endregion

    protected override void SetFacingDirection(float directionX)
    {
        if (directionX == 0) return;
        facingDirection = Mathf.Sign(directionX); // Agora atualiza corretamente
        base.SetFacingDirection(directionX);
    }

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        // Raio de detecção frontal
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Linha indicando a direção e distância do dash
        Gizmos.color = Color.red;
        Vector3 dashDir = new Vector3(facingDirection * dashDistance, 0f, 0f);
        Gizmos.DrawLine(transform.position, transform.position + dashDir);
    }
    #endregion
}