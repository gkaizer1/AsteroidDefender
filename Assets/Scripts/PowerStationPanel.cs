using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class PowerStationPanel : MonoBehaviour, ISelectionPanel
{
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public PowerProviderBehavior powerProvider;

    [Header("UI")]
    public TextMeshProUGUI powerGenerationText;
    public TextMeshProUGUI powerConsumedLabel;

    DateTime lastPowerConsumedUpdated = DateTime.Now;
    float powerConsumedCounter = 0.0f;

    public Slider powerSlider;
    private List<float> movingAverage = new List<float>();

    public void SetTarget(GameObject gameObject)
    {
        target = gameObject;

        powerProvider = target.GetComponentInChildren<PowerProviderBehavior>();
        powerProvider.OnPowerChanged.AddListener(OnPowerChanged);


        powerGenerationText.text = powerProvider.MWPerSecond.ToString("0");
    }

    public void OnDestroy()
    {
        if(powerProvider != null)
            powerProvider.OnPowerChanged.RemoveListener(OnPowerChanged);
    }

    public void OnPowerChanged(float current, float generated)
    {
        float powerPrecentage = current / generated;
        movingAverage.Add(powerPrecentage);
        if (movingAverage.Count > 20)
            movingAverage.RemoveAt(0);

        powerSlider.value = movingAverage.Average();

        powerConsumedCounter += (generated - current);
        if ((DateTime.Now - lastPowerConsumedUpdated).TotalSeconds > 1.0)
        {
            powerConsumedLabel.text = powerConsumedCounter.ToString("0");
            lastPowerConsumedUpdated = DateTime.Now;
            powerConsumedCounter = 0.0f;
        }
    }
}
