using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientBar : MonoBehaviour
{
    public Gradient gradient;
    [SerializeField]
    private Slider bar;
    [SerializeField]
    private Image fill;

    private void Start()
    {
        bar = GetComponent<Slider>();
        fill = bar.fillRect.gameObject.GetComponent<Image>();
    }

    public Color getColor(float value)
    {
        return gradient.Evaluate(value);
    }

    public void updateBar()
    {
        fill.color = getColor(bar.value/100);
    }
}
