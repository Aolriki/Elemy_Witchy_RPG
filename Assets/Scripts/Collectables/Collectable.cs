using UnityEngine;

public class Collectable : MonoBehaviour
{
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        Collect();
    }

    public virtual void Collect()
    {
        Destroy(gameObject);
    }
}
