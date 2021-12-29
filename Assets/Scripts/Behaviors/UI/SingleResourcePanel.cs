using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using DG.Tweening;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SingleResourcePanel : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public string text;
    public string postFix;

    Tween _tween = null;

    public Resource _resource = Resource.MONEY;

    // Update is called once per frame
    void Start()
    {
        if (textObject != null)
        {
            string localText = text + postFix;
            if (textObject.text != localText)
                textObject.text = localText;
        }

        if (ResourceManagerBehavior.Instance != null)
        {
            foreach (var resource in ResourceManagerBehavior.Instance.resource)
            {
                if (resource.resource == _resource)
                {
                    resource.OnAmountChanged.AddListener(OnResouceChanged);
                    OnResouceChanged(resource.Count, 0.0f);
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (ResourceManagerBehavior.Instance == null)
            return;

        foreach (var resource in ResourceManagerBehavior.Instance.resource)
        {
            if (resource.resource == _resource)
            {
                resource.OnAmountChanged.RemoveListener(OnResouceChanged);
                break;
            }
        }
    }

    public void OnResouceChanged(float newValue, float previousValue)
    {
        float currentValue = previousValue;
        float delta = Mathf.Abs(previousValue - newValue);
        if (delta < 3.0f)
            this.textObject.text = ((int)newValue).ToString("N0", CultureInfo.CurrentCulture);
        else
        {
            if (_tween != null)
                _tween.Complete(true);

            _tween = DG.Tweening.DOTween.To(
                () => currentValue,
                x =>
                {
                    currentValue = x;
                    this.textObject.text = ((int)currentValue).ToString("N0", CultureInfo.CurrentCulture);
                },
                newValue,
                0.5f
            ).OnComplete(() => _tween = null).OnKill(() => _tween = null);
        }

    }
}
