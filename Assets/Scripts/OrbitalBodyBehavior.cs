using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitalBodyBehavior : MonoBehaviour
{
    public float OrbitDistance = 15.0f;
    public float DegreePerSecond = 15.0f;
    [Range(-360f, 360f)]
    public float StartAngle = 0f;
    float _previousAngle = 0f;

    [Header("Children")]
    public List<GameObject> Children = new List<GameObject>();

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, DegreePerSecond * Time.fixedDeltaTime);
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var child in Children)
        {
            float x = Mathf.Cos(StartAngle * Mathf.Deg2Rad) * OrbitDistance;
            float y = Mathf.Sin(StartAngle * Mathf.Deg2Rad) * OrbitDistance;

            child.transform.position = new Vector3(x, y, 0f);
        }
        _previousAngle = StartAngle;
    }

#if UNITY_EDITOR
    public void Update()
    {
        if (_previousAngle != StartAngle)
            Init();
    }
#endif
}
