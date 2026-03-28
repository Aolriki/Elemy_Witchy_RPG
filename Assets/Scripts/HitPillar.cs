using UnityEngine;

public class HitPillar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInChildren<IDamageable>().Damage(1, transform.position, false);
        }
    }
}
