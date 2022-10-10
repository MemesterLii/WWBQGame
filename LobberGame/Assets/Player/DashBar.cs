using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashBar : MonoBehaviour
{
    [SerializeField] Slider dashSlider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;

    public void SetCooldown(int cooldownValue)
    {
        dashSlider.value = 30 - cooldownValue;

        fill.color = gradient.Evaluate(dashSlider.normalizedValue);
    }
    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
