using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBehavior : MonoBehaviour
{
    public float Acceleration = 0.5f;
    public float MaxSpeed = 1.0f;
    public float MaxRotationPerSecond = 45.0f;
    public bool StopHalfwayThrough = true;
    public bool StopAtTarget = true;
}
