using UnityEngine;

public class Musgolemy : EnemyClass
{
    [Header("Player Detection — Raycast")]
    public float detectionRange = 5f;
    public LayerMask playerLayer;
    public float detectionHeightOffset = 0.5f;

    [Header("Chase Timeout")]
    public float giveUpTime = 1f;

    [Header("Detection VFX")]
    public GameObject detectionIcon;

    private Transform playerTransform;
    private float outOfZoneTimer = 0f;

    private enum State { Patrolling, Following }
    private State currentState = State.Patrolling;

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
                TryDetectPlayer();
                break;

            case State.Following:
                ChasePlayer();
                if (!IsInsidePatrolZone(transform.position.x))
                {
                    outOfZoneTimer += Time.deltaTime;
                    if (outOfZoneTimer >= giveUpTime)
                        GiveUp();
                }
                else
                {
                    outOfZoneTimer = 0f;
                }
                break;
        }
    }

    private void TryDetectPlayer()
    {
        float dir = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 origin = (Vector2)transform.position + Vector2.up * detectionHeightOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * dir, detectionRange, playerLayer);

        if (hit.collider == null) return;

        playerTransform = hit.transform;
        currentState = State.Following;
        outOfZoneTimer = 0f;

        if (detectionIcon != null)
            detectionIcon.SetActive(true);
    }

    private void ChasePlayer()
    {
        if (playerTransform == null) { GiveUp(); return; }

        Vector2 target = new Vector2(playerTransform.position.x, transform.position.y);
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        SetFacingDirection(direction.x);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    private void GiveUp()
    {
        playerTransform = null;
        outOfZoneTimer = 0f;
        currentState = State.Patrolling;

        if (detectionIcon != null)
            detectionIcon.SetActive(false);

        float distToA = Mathf.Abs(transform.position.x - pointA.x);
        float distToB = Mathf.Abs(transform.position.x - pointB.x);
        targetPoint = distToA < distToB ? pointA : pointB;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        float dir = transform.localScale.x > 0 ? 1f : -1f;
        Vector3 origin = transform.position + Vector3.up * detectionHeightOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + new Vector3(dir * detectionRange, 0f, 0f));
    }
}