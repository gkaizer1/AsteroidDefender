using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGrid : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public static int POINT_COUNT = 72;
    // Start is called before the first frame update
    void Awake()
    {
        for (int r = 9; r <= 21; r += 2)
        {
            DrawCircle((float)r);
        }
    }

    void DrawCircle(float radius)
    {
        LineRenderer _newLine = Instantiate(lineRenderer, this.transform);
        _newLine.name = $"grid_line_{radius}";
        List<Vector3> positions = new List<Vector3>((POINT_COUNT + 1) * 5);
        int idx = 0;
        float degPerTick = (360.0f / POINT_COUNT);
        for (float angle = 0; angle <= 360; angle = Utils.AngleToNearestValid(angle + degPerTick, radius))
        {
            float nextAngle = Utils.NextValidAngle(angle, radius);
            float nextRadius = radius + 2f;
            positions[idx++] = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 1f);
            positions[idx++] = new Vector3(nextRadius * Mathf.Cos(angle * Mathf.Deg2Rad), nextRadius * Mathf.Sin(angle * Mathf.Deg2Rad), 1f);

            positions[idx++] = new Vector3(nextRadius * Mathf.Cos(nextAngle * Mathf.Deg2Rad), nextRadius * Mathf.Sin(nextAngle * Mathf.Deg2Rad), 1f);
            positions[idx++] = new Vector3(radius * Mathf.Cos(nextAngle * Mathf.Deg2Rad), radius * Mathf.Sin(nextAngle * Mathf.Deg2Rad), 1f);
            
            positions[idx++] = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 1f);
        }
        _newLine.positionCount = positions.Count;
        _newLine.SetPositions(positions.ToArray());
    }
}
