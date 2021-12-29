using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public float Max = 100.0f;
    public float Current = 100.0f;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = Max;
        slider.value = 50.0f;
    }
}
