using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuUi : MonoBehaviour
{
    [SerializeField] private Button[] menuButtons;

    private GameObject lastSelected;

    private void OnEnable()
    {
        HideAllImages();
    }

    private void Update()
    {
        GameObject selected = EventSystem.current?.currentSelectedGameObject;

        if (selected == lastSelected)
            return;

        lastSelected = selected;
        RefreshImages(selected);
    }

    private void RefreshImages(GameObject selected)
    {
        foreach (Button button in menuButtons)
        {
            if (button == null) continue;

            Image img = button.GetComponent<Image>();
            if (img == null) continue;

            img.enabled = button.gameObject == selected;
        }
    }

    private void HideAllImages()
    {
        foreach (Button button in menuButtons)
        {
            if (button == null) continue;

            Image img = button.GetComponent<Image>();
            if (img != null)
                img.enabled = false;
        }

        lastSelected = null;
    }
}
