using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class DropSettings
{
    [Header("Settings")]
    [Range(0f, 1f)]
    public float DropChance = 0.1f;

    [Header("Prefabs")]
    public GameObject ResourcePrefab;
}

[Serializable]
public class AlwaysDrop
{
    [Header("Settings")]
    public float Amount = 1f;
    public Resource resource;

    [Header("Prefabs")]
    public GameObject ResourcePrefab;
}

public class DropOnDeathBehavior : MonoBehaviour
{
    public float dropRange = 1.0f;
    public List<DropSettings> dropSettings = new List<DropSettings>();
    public List<AlwaysDrop> alwaysDrop = new List<AlwaysDrop>();

    void DropResourceAlways()
    {
        int count = 0;
        foreach (var drop in alwaysDrop)
        {
            var _obj = Instantiate(drop.ResourcePrefab);
            _obj.gameObject.transform.position = this.transform.position + (Vector3.up * count++);
            _obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"+{drop.Amount}";
            ResourceManagerBehavior.Instance.AddResource(drop.resource, drop.Amount);
        }
    }

    public void OnDestroy()
    {
        DropResourceAlways();

        dropSettings.ForEach(dropSetting =>
        {
            float roll = UnityEngine.Random.Range(0f, 1f);
            if (roll > dropSetting.DropChance)
                return;

            var resource = Instantiate(dropSetting.ResourcePrefab, AstroidsContainer.Instance.transform);

            float dropRadius = 0.5f + UnityEngine.Random.Range(0f, dropRange);

            float randomAngle = UnityEngine.Random.Range(0, 360.0f) * Mathf.Deg2Rad;
            float xStartPos = Mathf.Cos(randomAngle) * dropRadius;
            float yStartPos = Mathf.Sin(randomAngle) * dropRadius;

            resource.transform.position = this.transform.position;
            var finalPosition = this.transform.position + new Vector3(xStartPos, yStartPos, 0f);
            resource.transform.DOMove(finalPosition, 1.0f);
        });
    }
}
