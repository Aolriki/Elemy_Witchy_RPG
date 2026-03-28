using UnityEngine;
public class EnemyClass : MonoBehaviour
{
    #region Headers
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Transform pointA;
    public Transform pointB;
    protected Transform targetPoint;
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
        StartPatrol();
    }
    protected virtual void Update()
    {
        if (pointA != null && pointB != null)
            MoveBetweenPoints();
    }
    #region Patrulha
    protected void StartPatrol()
    {
        if (pointA != null && pointB != null)
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
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        SetFacingDirection(direction.x);
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = (targetPoint == pointB) ? pointA : pointB;
            isWaiting = true;
            waitTimer = waitTime;
        }
    }
    #endregion
    protected virtual void SetFacingDirection(float directionX)
    {
        if (directionX == 0) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX > 0 ? 1 : -1);
        transform.localScale = scale;
    }
    #region Dano e Morte
    /*protected virtual void Die()
    {
        Debug.Log(gameObject.name + " morreu.");
        if (loot1 != null)
            Instantiate(loot1, transform.position, Quaternion.identity);
        //animação de morte aqui. Puxa o Destroy() no ultimo frame.
        Destroy(gameObject);
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
    }
    */
    #endregion
    #region Contact Damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInChildren<IDamageable>().Damage(damage, transform.position, false);
        }
    }
    #endregion
}