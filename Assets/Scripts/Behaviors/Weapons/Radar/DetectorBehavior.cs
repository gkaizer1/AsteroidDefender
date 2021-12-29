using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class DetectorBehavior : MonoBehaviour, IStateMachine
{
    public GameObject circleChild;
    public float range = 5.0f;
    public float detectionStrength = 1.0f;
    public float requiredPower = 20.0f;

    PowerConsumer _powerConsumer = null;
    public string CurrentState => "Scanning";

    // Start is called before the first frame update
    void Start()
    {
        circleChild.SetActive(false);
        _powerConsumer = GetComponent<PowerConsumer>();
    }

    public void UpdateDetectedItems()
    {
        var allDetectableObjects = GameObject.FindObjectsOfType<DetectableBehavior>();
        foreach(var detectable in allDetectableObjects)
        {
            float distance = (detectable.transform.position - this.transform.position).magnitude;
            if (distance > range)
                return;

            // Anything within 10 range is AUTO detected
            if(distance < 10.0f)
            {
                detectable.SetDetectedPrecentage(1.0f);
                return;
            }

            // Detection strength is:
            // How far is the object * detectionStrength
            float rangePrecentage = 1.0f - Mathf.Abs(distance / range);
            float detection = (detectionStrength / 100.0f) * rangePrecentage;

            detectable.SetDetectedPrecentage(Mathf.Clamp01(detectable.DetectedPrecentage + detection));
        }
    }

    public void OnFire()
    {
        if (_powerConsumer == null || _powerConsumer.CurrentPower < requiredPower)
            return;

        _powerConsumer.CurrentPower -= requiredPower;

        circleChild.transform.localScale = Vector3.zero;
        circleChild.gameObject.SetActive(true);

        float timeBetweenFiring = 1.0f / this.GetComponent<AutoFireBehavior>().fireRate;
        var lineRenderer = circleChild.GetComponent<LineRenderer>();
        var circleBehavior = circleChild.GetComponent<CircleBehavior>();
        float tweenTime = timeBetweenFiring / 1.1f;
        lineRenderer.startColor = circleBehavior.color;
        lineRenderer.endColor = circleBehavior.color;
        
        // tween color
        Color endColor = new Color(circleBehavior.color.r, circleBehavior.color.g, circleBehavior.color.b, 0.0f);
        lineRenderer.DOColor(new Color2(circleBehavior.color, circleBehavior.color), new Color2(endColor, endColor), tweenTime).SetRecyclable(true);
        // tween scale
        circleChild.transform.DOScale(range, tweenTime).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            // Hide/disable the circle
            circleChild.gameObject.SetActive(false);
            UpdateDetectedItems();
        }).SetRecyclable(true);
    }

    public void OnPowerChanged(float power, float maxPower)
    {

    }

    public void OnAmmoChanged(float currentAmmo, float maxAmmo)
    {

    }
}
