using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorBehavior : MonoBehaviour
{
    public float DegreePerSecond = 15.0f;
    public float _timeSinceLastUpdate = 0.0f;
    public float updateFrequency = 0.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        _timeSinceLastUpdate += Time.fixedDeltaTime;
        if (_timeSinceLastUpdate > updateFrequency)
        {
            transform.Rotate(Vector3.forward, DegreePerSecond * _timeSinceLastUpdate);
            _timeSinceLastUpdate = 0.0f;
        }
    }
}
