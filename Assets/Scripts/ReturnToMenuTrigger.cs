using UnityEngine;

public class ReturnToMenuTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Trigger ativado!");
            ScreenManager.Instance.ReturnToMainMenu();
        }
    }
}