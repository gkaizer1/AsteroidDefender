using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectableBehavior : MonoBehaviour
{
    public GameObject NotDetectedSprite;
    public UnityEvent OnDetectedCallback;
    public float DetectedPrecentage = 0.0f;

    [Range(0.0f, 100.0f)]
    public float UpdateTime = 5.0f;

    public bool IsDetected
    {
        get
        {
            return DetectedPrecentage >= 1.0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // This object is starting detected - let deps know
        if (IsDetected)
            OnDetectedCallback?.Invoke();
    }


    /// <summary>
    /// Detected between 0.0 - 1.0
    /// </summary>
    /// <param name="precentage"></param>
    public void SetDetectedPrecentage(float precentage)
    {
        if (precentage == DetectedPrecentage)
            return;

        DetectedPrecentage = precentage;
        if (precentage >= 1.0f)
        {
            OnDetectedCallback?.Invoke();
        }
        else
        {
            // If this object is in viewe then show the indicator
            if (Utils.GetOrthographicBounds(Camera.main).Contains(this.transform.position))
            {
                var indicator = Instantiate(NotDetectedSprite, AstroidsContainer.Instance != null ? AstroidsContainer.Instance.transform : null);
                indicator.transform.position = this.transform.localPosition;

                // On low zooms show a nice fadein/fade out - otherwise it's just a waste in large zooms
                if (Camera.main.orthographicSize < 20.0f)
                {
                    var spriteRenderer = indicator.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.0f);
                    indicator.GetComponent<SpriteRenderer>().DOFade(1.0f, 1.0f).SetAutoKill(true).OnComplete(() =>
                    {
                        indicator.GetComponent<SpriteRenderer>().DOFade(0.0f, 1.0f).OnComplete(() =>
                        {
                            Destroy(indicator);
                        });
                    });
                }
                else
                {
                    Destroy(indicator, 2.0f);
                }
            }

        }
    }
}
