using UnityEngine;
using UnityEngine.UI;

public class ElementTimerUI : MonoBehaviour
{
    [Header("Sliders")]
    public Image ignaImage;
    public Image floraImage;
    public Image aquaImage;

    private Player player;
    private Image activeImage;

    private void Start()
    {
        player = Player.instance;

        ignaImage.gameObject.SetActive(false);
        floraImage.gameObject.SetActive(false);
        aquaImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (player.currentElement == EElements.None)
        {
            if (activeImage != null)
            {
                activeImage.gameObject.SetActive(false);
                activeImage = null;
            }
            return;
        }

        
        Image targetImage = player.currentElement switch
        {
            EElements.Igna => ignaImage,
            EElements.Flora => floraImage,
            EElements.Aqua => aquaImage,
            _ => null
        };

        if (targetImage != activeImage)
        {
            if (activeImage != null) activeImage.gameObject.SetActive(false);
            activeImage = targetImage;
            if (activeImage != null) activeImage.gameObject.SetActive(true);
        }

        if (activeImage != null)
        {
            float normalized = Mathf.Clamp01(player.ElementTimeRemaining / player.elemyDuration);
            activeImage.fillAmount = normalized; // 1 = cheio, 0 = vazio
        }
    }
}

