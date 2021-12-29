using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerStatusPanel : MonoBehaviour, ISelectionPanel
{
    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public PowerConsumer powerConsumerBehavior;

    [Header("UI")]
    public TextMeshProUGUI powerValueLabel;

    public void SetTarget(GameObject gameObject)
    {
        target = gameObject;

        powerConsumerBehavior = target.GetComponent<PowerConsumer>();
        if (powerConsumerBehavior != null)
        {
            powerConsumerBehavior.OnPowerChanged.AddListener(OnPowerChanged);
            OnPowerChanged(powerConsumerBehavior.CurrentPower, powerConsumerBehavior.MaxPower);
        }
    }

    public void OnDestroy()
    {
        if (powerConsumerBehavior != null)
            powerConsumerBehavior.OnPowerChanged.RemoveListener(OnPowerChanged);
    }

    public void OnPowerChanged(float current, float max)
    {
        powerValueLabel.text = $"{current.ToString("0")}/{max.ToString("0")}";
    }
}
