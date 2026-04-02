using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    #region Headers
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Patrol Zone")]
    [Tooltip("BoxCollider2D (trigger) que define a área de patrulha. Os pontos A e B são gerados automaticamente.")]
    public BoxCollider2D patrolZone;

    // Pontos gerados automaticamente a partir do patrolZone
    protected Vector2 pointA;
    protected Vector2 pointB;
    protected Vector2 targetPoint;

    // Bounds da zona (calculados uma vez no Start)
    protected float zoneMinX;
    protected float zoneMaxX;
    #endregion

    #region Componentes
    protected Rigidbody2D rb;
    protected Collider2D damageArea;
    [SerializeField] protected int damage = 1;
    #endregion

    #region Timer
    private float waitTimer = 0f;
    public float waitTime = 1f;
    private bool isWaiting = false;
    #endregion

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        damageArea = GetComponentInChildren<Collider2D>();

        if (patrolZone != null)
        {
            // Gera A e B nas bordas esquerda e direita do BoxCollider2D
            Bounds b = patrolZone.bounds;
            pointA = new Vector2(b.min.x, transform.position.y);
            pointB = new Vector2(b.max.x, transform.position.y);
            zoneMinX = b.min.x;
            zoneMaxX = b.max.x;
        }
        else
        {
            // Fallback: fica parado se não tiver zona configurada
            Debug.LogWarning($"{gameObject.name}: PatrolZone não atribuído!", this);
            zoneMinX = transform.position.x - 0.1f;
            zoneMaxX = transform.position.x + 0.1f;
        }

        StartPatrol();
    }

    protected virtual void Update()
    {
        MoveBetweenPoints();
    }

    #region Patrulha
    protected void StartPatrol()
    {
        targetPoint = pointA;
    }

    protected void MoveBetweenPoints()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
                isWaiting = false;
            return;
        }

        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        SetFacingDirection(direction.x);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPoint,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = (targetPoint == pointB) ? pointA : pointB;
            isWaiting = true;
            waitTimer = waitTime;
        }
    }

    /// <summary>
    /// Retorna true se a posição X dada está dentro da zona de patrulha.
    /// </summary>
    protected bool IsInsidePatrolZone(float posX)
    {
        return posX >= zoneMinX && posX <= zoneMaxX;
    }
    #endregion

    protected virtual void SetFacingDirection(float directionX)
    {
        if (directionX == 0) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX > 0 ? 1 : -1);
        transform.localScale = scale;
    }

    #region Contact Damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject
                .GetComponentInChildren<IDamageable>()
                .Damage(damage, transform.position, false);
        }
    }
    #endregion

    #region Gizmos
    protected virtual void OnDrawGizmosSelected()
    {
        if (patrolZone == null) return;

        Bounds b = patrolZone.bounds;
        float y = Application.isPlaying ? pointA.y : transform.position.y;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(b.min.x, y), new Vector3(b.max.x, y));
        Gizmos.DrawSphere(new Vector3(b.min.x, y), 0.15f); // ponto A
        Gizmos.DrawSphere(new Vector3(b.max.x, y), 0.15f); // ponto B
    }
    #endregion
}