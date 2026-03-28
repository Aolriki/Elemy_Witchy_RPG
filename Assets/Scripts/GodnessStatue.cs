using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class GodnessStatue : MonoBehaviour
{
    public GameObject flowers;
    public Transform statueSpawnPoint;
    public bool interacted = false;

    // Invocação configurável pelo Inspector
    [Header("Events")]
    public UnityEvent onPlayerInteract;

    public static System.Action<GodnessStatue> OnStatueActivated;

    private void OnEnable()
    {
        OnStatueActivated += HandleOtherStatueActivated;
    }

    private void OnDisable()
    {
        OnStatueActivated -= HandleOtherStatueActivated;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !interacted)
        {
            OnStatueActivated?.Invoke(this);
            CheckPoint();
            FlowersAnim();
            interacted = true;
            AudioManager.Instance.PlayEffect(SFXID.GoodnessStatue);

            // Dispara os callbacks configurados no Inspector
            onPlayerInteract?.Invoke();
        }
    }

    private void HandleOtherStatueActivated(GodnessStatue activatedStatue)
    {
        if (activatedStatue != this)
        {
            interacted = false;
            flowers.transform.DOScale(0f, 0.3f)
                .OnComplete(() => flowers.SetActive(false));
        }
    }

    private void CheckPoint()
    {
        LevelManager.instance.lastSafePlace = statueSpawnPoint;
    }

    private void FlowersAnim()
    {
        flowers.SetActive(true);
        flowers.transform.localScale = Vector3.zero;
        flowers.transform.DOScale(1f, 0.5f);
    }
}