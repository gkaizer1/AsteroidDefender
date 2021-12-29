using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthTilePanelBehavior : MonoBehaviour, ISelectionPanel
{
    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public HealthBehavior healthBehavior;

    [Header("UI")]
    public TextMeshProUGUI healthLabel;

    public void SetTarget(GameObject gameObject)
    {
        target = gameObject;

        healthBehavior = target.GetComponent<HealthBehavior>();
        if (healthBehavior != null)
        {
            healthBehavior.OnHealthChanged.AddListener(OnHealthChanged);
            OnHealthChanged(healthBehavior.Health, healthBehavior.MaxHealth);
        }
    }

    public void OnDestroy()
    {
        if (healthBehavior != null)
            healthBehavior.OnHealthChanged.RemoveListener(OnHealthChanged);
    }

    public void OnHealthChanged(float current, float max)
    {
        healthLabel.text = $"{current.ToString("0")}/{max.ToString("0")}";
    }
}
