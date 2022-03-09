using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResourceTextBehavior : MonoBehaviour
{
    public Resource resource;
    public float minimum = 100;
    public TMPro.TextMeshProUGUI label;
    public Color hasEnoughResource = Color.green;
    public Color notEnoughResource = Color.red;

    public UnityEvent OnResourceRequirnmentReached;

    void Start()
    {
        var resourceItem = ResourceManagerBehavior.Instance.resource.Find(x => x.resource == resource);
        resourceItem.OnAmountChanged.AddListener(OnResourceAdded);
        OnResourceAdded(resourceItem.Count, 0);
    }

    private void OnDestroy()
    {
        if (ResourceManagerBehavior.Instance == null)
            return;

        ResourceManagerBehavior.Instance.resource.Find(x => x.resource == resource).OnAmountChanged.RemoveListener(OnResourceAdded);
    }

    public void OnEnable()
    {
        if (ResourceManagerBehavior.Instance == null)
            return;

        var resourceItem = ResourceManagerBehavior.Instance.resource.Find(x => x.resource == resource);
        OnResourceAdded(resourceItem.Count, 0);
    }

    public void OnResourceAdded(float current, float previous)
    {
        if (!this.gameObject.activeInHierarchy)
            return;

        if (label != null)
        {
            if (current >= minimum)
            {
                label.color = hasEnoughResource;
                OnResourceRequirnmentReached?.Invoke();
            }
            else
                label.color = notEnoughResource;

        }
    }
}
