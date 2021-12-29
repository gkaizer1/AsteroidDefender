using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PowerLineInformation
{
    public GameObject lineRenderer;
    public GameObject target;
}

public class PowerSelectedBehavior : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject lineObjectPrefab;

    PowerProviderBehavior _powerProvider;
    List<PowerLineInformation> _lines = new List<PowerLineInformation>();

    float tweenPrecentage = 0.0f;
    DateTime _dtLastTweenUpdate;

    private void OnEnable()
    {
        _powerProvider = GetComponent<PowerProviderBehavior>();
        foreach(var x in _powerProvider.powerConsumers)
        {
            if (lineObjectPrefab == null)
                return;
            
            var obj = Instantiate(lineObjectPrefab, this.transform);
            obj.name = "power_indicator";

            List<Vector3> pos = new List<Vector3>();
            pos.Add(this.transform.position);
            pos.Add(this.transform.position);
            var lineRenderer = obj.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(pos.ToArray());

            _lines.Add(new PowerLineInformation()
            {
                lineRenderer = obj,
                target = x.gameObject
            });
        }

        // Reset tween parameters
        tweenPrecentage = 0.0f;
        _dtLastTweenUpdate = DateTime.UtcNow;
    }

    private void OnDisable()
    {
        _lines.ForEach(x =>
        {
            Destroy(x.lineRenderer);
        });
        _lines.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (tweenPrecentage < 1.0f)
        {
            // Use datetime in order for this animation to work when paused
            float secondsElapsed = (float)(DateTime.UtcNow - _dtLastTweenUpdate).TotalSeconds;
            float tweenTimeSeconds = 0.25f;
            tweenPrecentage = Mathf.Clamp01(tweenPrecentage + 1.0f / tweenTimeSeconds * secondsElapsed);
            _dtLastTweenUpdate = DateTime.UtcNow;
        }

        _lines.ForEach(x =>
        {
            if (!x.target)
                return;

            var lineRenderer = x.lineRenderer.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, this.transform.position);
            if(tweenPrecentage >= 1.0f)
                lineRenderer.SetPosition(1, x.target.transform.position);
            else
            {
                var deltaVector = (x.target.transform.position - this.transform.position);
                float newMagnitude = (deltaVector.magnitude * tweenPrecentage);
                lineRenderer.SetPosition(1, this.transform.position + (deltaVector.normalized * newMagnitude));
            }
        });
    }
}
