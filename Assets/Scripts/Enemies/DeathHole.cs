using UnityEngine;
public class DeathHole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponentInChildren<PlayerHealth>();
            if (health != null)
                health.Death();
        }
    }
}