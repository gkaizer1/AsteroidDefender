using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum Resource
{
    MONEY,
    METAL,
    PEOPLE,
    URANIUM,
    UNOBTANIUM,
    SHUTTLES
}

[Serializable]
public class ResourceInfo
{
    public UnityEvent<float, float> OnAmountChanged;

    public Resource resource;
    public float _Count = 0.0f;
    public float IncomePerSecond = 1.0f;
    public float Count
    {
        get
        {
            return _Count;
        }
        set
        {
            if (value == _Count)
                return;

            var previousValue = _Count;
            _Count = value;

            OnAmountChanged?.Invoke(_Count, previousValue);
        }
    }
}

public class ResourceManagerBehavior : MonoBehaviour
{
    public List<ResourceInfo> resource = new List<ResourceInfo>();

    Coroutine _resourceRoutine;

    public static ResourceManagerBehavior Instance = null;
    public UnityEvent<Resource, float> OnResourceChanged;

    // Start is called before the first frame update
    void Awake()
    {
        _resourceRoutine = StartCoroutine(UpdateResources());
        if(Instance != null)
        {
            Debug.LogError("Duplicate resource manager");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        Instance = this;
    }

    IEnumerator UpdateResources()
    {
        while (this.gameObject != null)
        {
            resource.ForEach(x =>
            {
                AddResource(x.resource, x.IncomePerSecond);
            });
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void AddResource(Resource resourceToAdd, float amount)
    {
        resource.ForEach(x =>
        {
            if (x.resource != resourceToAdd)
                return;

            x.Count += amount;
            OnResourceChanged?.Invoke(resourceToAdd, x.Count);
        });
    }
}
