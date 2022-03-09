using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class ResourceRequirnment
{
    public Resource resource;
    public float minimum;
    public TMPro.TextMeshProUGUI resourceText;
}

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class ResourceBoundButtonBehavior : MonoBehaviour
{
    public Button button;
    public TMPro.TextMeshProUGUI indicatorText;
    public List<ResourceRequirnment> resourceRequirnments = new List<ResourceRequirnment>();
    public UnityEvent ActionOnSpent;

    void Start()
    {
        if (button == null)
            button = this.GetComponent<Button>();

        if(button == null)
            gameObject.UnassignedReference(nameof(button));

        OnResourceChanged(Resource.PEOPLE, 0);

        ResourceManagerBehavior.Instance.OnResourceChanged.AddListener(OnResourceChanged);
    }

    private void OnDestroy()
    {
        ResourceManagerBehavior.Instance.OnResourceChanged.RemoveListener(OnResourceChanged);
    }

    public void OnResourceChanged(Resource resource, float currentAmount)
    {
        // !! Do NOT update the button if thhis component is disbable !!
        if (!this.enabled || !this.gameObject.activeInHierarchy)
            return;

        bool hasEnough = true;
        foreach(var req in resourceRequirnments)
        {
            float resourceCount = ResourceManagerBehavior.Instance.resource.Find(x => x.resource == req.resource).Count;
            bool hasEnoughResource = req.minimum <= resourceCount;
            hasEnough = hasEnough && hasEnoughResource;
            if (req.resourceText != null)
            {
                req.resourceText.color = hasEnoughResource ? Color.green : Color.red;
            }
        }

        if (indicatorText != null)
        {
            indicatorText.text = hasEnough ? "" : "Can't afford";
            indicatorText.color = Color.red;
        }

        button.interactable = hasEnough;
    }

    public void Spend()
    {
        foreach (var req in resourceRequirnments)
        {
            float resourceCount = ResourceManagerBehavior.Instance.resource.Find(x => x.resource == req.resource).Count;
            if (resourceCount >= req.minimum)
            {
                ResourceManagerBehavior.Instance.AddResource(req.resource, -req.minimum);
            }
        }
        ActionOnSpent?.Invoke();
    }

#if UNITY_EDITOR
    public void Update()
    {
        foreach (var req in resourceRequirnments)
        {
            string text = (req.resource == Resource.MONEY ? "$" : "") + req.minimum.ToString();
            if (req.resourceText != null)
            {
                if(req.resourceText.text != text)
                {
                    req.resourceText.text = text;
                    req.resourceText.color = Color.green;
                }
            }
        }


    }
#endif
}
