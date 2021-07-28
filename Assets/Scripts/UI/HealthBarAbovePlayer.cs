using UnityEngine;
using UnityEngine.UI;

public class HealthBarAbovePlayer : MonoBehaviour
{
    private Slider slider;
    private Image fill;
    [SerializeField] private Gradient gradient;

    // Start is called before the first frame update
    void Start()
    {
        slider = transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        fill = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();
    }

    public void SetMaxHealth(int maxHealth, Gradient gradient)
    {
        slider.maxValue = maxHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    
    public void SetHealthBar(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
