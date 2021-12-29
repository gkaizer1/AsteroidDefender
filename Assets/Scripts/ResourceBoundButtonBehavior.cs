using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResourceRequirnment
{
    public Resource resource;
    public float minimum;
}

public class ResourceBoundButtonBehavior : MonoBehaviour
{
    public Button button;
    public TMPro.TextMeshProUGUI indicatorText;
    public List<ResourceRequirnment> resourceRequirnments = new List<ResourceRequirnment>();

    void Start()
    {
        if (button == null)
            button = this.GetComponent<Button>();

        if(button == null)
            gameObject.UnassignedReference(nameof(button));

        ResourceManagerBehavior.Instance.OnResourceChanged.AddListener(OnResourceChanged);
    }

    private void OnDestroy()
    {
        ResourceManagerBehavior.Instance.OnResourceChanged.RemoveListener(OnResourceChanged);
    }

    public void OnResourceChanged(Resource resource, float currentAmount)
    {
        if (!this.enabled || !this.gameObject.activeInHierarchy)
            return;

        foreach(var req in resourceRequirnments)
        {
            float resourceCount = ResourceManagerBehavior.Instance.resource.Find(x => x.resource == req.resource).Count;
            if (resourceCount < req.minimum)
            {
                button.interactable = false;

                if (indicatorText != null)
                {
                    indicatorText.text = "Can't afford";
                    indicatorText.color = Color.red;                    
                }

                return;
            }
        }

        if(!button.interactable)
            button.interactable = true;
        if (indicatorText != null)        
            indicatorText.text = "";        
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
    }
}
