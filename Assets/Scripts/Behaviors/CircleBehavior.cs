using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircleBehavior : MonoBehaviour
{
    public LineRenderer _lineRenderer;
    float _lastminAngle = 0.0f;
    float _lastAngle = 0.0f;
    float _lastRadius = 0f;
    public float _lastmindRadius = -1.0f;

    [Range(0.0f, 500f)] 
    public float width = 1.0f;
    public Color color = Color.red;
    public float startRadius = -1.0f;
    public float radius = 1.0f;

    [Range(-360.0f, 360)]
    public float MinAngle = 0f;
    [Range(-360.0f, 360)]
    public float MaxAngle = 360.0f;

    public int pointCount = 50;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        UpdateLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastAngle != MaxAngle || _lastRadius != radius || _lastminAngle != MinAngle || _lastmindRadius!= startRadius)
            UpdateLine();
    }
    
    void UpdateLine()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 pos;
        float anglePerPoint = (MaxAngle - MinAngle) / (float)pointCount;
        float r = radius;
        for (int i = 0; i <= pointCount; i++)
        {
            float angle = (MinAngle + anglePerPoint * i) * Mathf.Deg2Rad;
            if(startRadius >= 0)
            {
                r = startRadius + ((radius - startRadius) / pointCount) * i;
            }
            float x = r * Mathf.Cos(angle);
            float y = r * Mathf.Sin(angle);
            pos = new Vector3(x, y, 0);
            points.Add(pos);
        }
        if(MaxAngle == 360f)
            points.Add(points[1]);

        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        _lineRenderer.widthMultiplier = width;
        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());

        _lastAngle = MaxAngle;
        _lastRadius = radius;
        _lastminAngle = MinAngle;
        _lastmindRadius = startRadius;
    }
}
