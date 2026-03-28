using UnityEngine;

public class PrimalElementCollectable : Collectable
{
    [SerializeField] EElements element;

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))

            if (Player.instance.currentElement != EElements.None)

                return;

            else
                Collect();

    }

    public override void Collect()
    {
        Player.instance.currentElement = element;
        Player.instance.SetElemy(element);
        Debug.Log("Element Collected: " + element.ToString());
        base.Collect();

        AudioManager.Instance.PlayEffect(SFXID.ElemyCollect);
    }
}
