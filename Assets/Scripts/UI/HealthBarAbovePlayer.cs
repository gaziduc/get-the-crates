using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarAbovePlayer : MonoBehaviour
{
    private Slider slider;
    private Image fill;
    [SerializeField] private Gradient gradient;
    private float realHealth;

    // Start is called before the first frame update
    void Start()
    {
        slider = transform.GetChild(1).GetChild(0).GetComponent<Slider>();
        fill = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
    }

    public void SetMaxHealth(int maxHealth, Gradient gradient)
    {
        slider.maxValue = maxHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    
    public void SetHealthBar(int health)
    {
        realHealth = health;
    }

    private void Update()
    {
        if (realHealth < slider.value)
        {
            slider.value -= (slider.value - realHealth) * 10 * Time.deltaTime;
            if (slider.value <= realHealth + 0.01f)
                slider.value = realHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
        else if (realHealth > slider.value)
        {
            slider.value += (realHealth - slider.value) * 10 * Time.deltaTime;
            if (slider.value >= realHealth - 0.01f)
                slider.value = realHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
