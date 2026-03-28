using UnityEngine;
using UnityEngine.UI;

public class HealthPoint : MonoBehaviour
{
    [SerializeField] private Image healthImage;
    [SerializeField] private Color hasHealthColor = Color.red;
    [SerializeField] private Color hasNoHealthColor = Color.blue;
    public bool _hasHealth = false;


    public void SetHealth(bool hasHealth)
    {
        Debug.Log("SetHealth" + hasHealth);
        healthImage.gameObject.SetActive(true);
        if (hasHealth)
        {
            healthImage.color = hasHealthColor;
        }
        else
        {
            healthImage.color = hasNoHealthColor;
        } 
    }
}
