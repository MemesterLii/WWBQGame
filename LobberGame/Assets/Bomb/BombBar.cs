using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombBar : MonoBehaviour
{
    [SerializeField] Slider bombSlider;
    [SerializeField] Gradient gradient;
    public Image fill;

    public void SetCooldown(int cooldownValue)
    {
        bombSlider.value = 30 - cooldownValue;

        fill.color = gradient.Evaluate(bombSlider.normalizedValue);
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
