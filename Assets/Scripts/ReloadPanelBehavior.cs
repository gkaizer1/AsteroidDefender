using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReloadPanelBehavior : MonoBehaviour, ISelectionPanel
{
    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public ReloaderBehavior reloader;

    [Header("UI")]
    public TextMeshProUGUI reloadValueLabel;

    public void SetTarget(GameObject gameObject)
    {
        target = gameObject;
        reloader = target.GetComponent<ReloaderBehavior>();
        if (reloader != null)
        {
            reloader.onAmmoChangedCallBack.AddListener(onAmmoChanged);
            onAmmoChanged(reloader.Ammo, reloader.maxAmmo);
        }
    }

    public void OnDestroy()
    {
        if (reloader != null)
            reloader.onAmmoChangedCallBack.RemoveListener(onAmmoChanged);
    }

    public void onAmmoChanged(float current, float max)
    {
        reloadValueLabel.text = $"{current.ToString("0")}/{max.ToString("0")}";
    }
}
