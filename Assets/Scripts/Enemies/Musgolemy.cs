using UnityEngine;

public class Musgolemy : EnemyClass
{
    [Header("Player Detection")]
    public float detectionRange = 5f;
    public float chaseRange = 7f;
    private Transform playerTransform;

    [Header("Detection VFX")]
    public GameObject detectionIcon;

    private enum State { Patrolling, Following }
    private State currentState = State.Patrolling;


    private LayerMask playerLayer;
    protected override void Start()
    {
        base.Start(); // Roda o Start() da classe base
        playerLayer = LayerMask.GetMask("Player"); // Busca a layer "Player" automaticamente
    }

    protected override void Update()
    {
        DetectPlayer();

        switch (currentState)
        {
            case State.Patrolling:
                base.Update();
                break;

            case State.Following:
                ChasePlayer();
                break;
        }
    }

    #region Detecção e Perseguição
    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        switch (currentState)
        {
            case State.Patrolling:
                if (hit == null) break;

                Debug.Log("Player detected — following!");
                playerTransform = hit.transform;
                currentState = State.Following;
                if (detectionIcon != null) detectionIcon.SetActive(true);
                break;

            case State.Following:
                if (hit != null)
                {
                    playerTransform = hit.transform;
                    break;
                }

                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                if (distanceToPlayer > chaseRange)
                {
                    Debug.Log("Player out of range — returning to patrol.");
                    playerTransform = null;
                    currentState = State.Patrolling;
                    if (detectionIcon != null) detectionIcon.SetActive(false);
                    StartPatrol();
                }
                break;
        }
    }

    private void ChasePlayer()
    {
        if (playerTransform == null) return;

        // Alvo apenas no eixo X, mantendo o Y atual do inimigo
        Vector2 target = new Vector2(playerTransform.position.x, transform.position.y);
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        SetFacingDirection(direction.x);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
    #endregion
}
