using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommercialDistrictBehavior : MonoBehaviour
{
    [Header("Children")]
    public GameObject onAddResourcePanel;
    public Transform moneySpawnPoint;

    [Header("Resource Settings")]
    public Resource resource = Resource.MONEY;
    public float resourcePerTick = 10f;

    public void Fire()
    {
        var _obj = Instantiate(onAddResourcePanel);
        _obj.gameObject.transform.position = moneySpawnPoint.position; // this.transform.position + this.transform.up * 0.5f;// Camera.main.WorldToScreenPoint(this.transform.position);
        _obj.GetComponentInChildren<TextMeshProUGUI>().text = "+" + resourcePerTick;
        ResourceManagerBehavior.Instance.AddResource(resource, resourcePerTick);
    }
}
