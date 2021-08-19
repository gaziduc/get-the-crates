using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBarAbovePlayer : MonoBehaviour
{
    private Slider slider;
    private bool isReloading = false;

    // Start is called before the first frame update
    void Start()
    {
        slider = transform.GetChild(2).GetChild(1).GetComponent<Slider>();
    }

    private void Update()
    {
        if (isReloading)
        {
            slider.value += Time.deltaTime;
            if (slider.value >= slider.maxValue)
                isReloading = false;
        }
    }

    public void Shoot(float reloadTime)
    {
        slider.value = 0;
        slider.maxValue = reloadTime;
        isReloading = true;
    }

    public void SetToReloaded()
    {
        slider.value = slider.maxValue;
        isReloading = false;
    }
}
