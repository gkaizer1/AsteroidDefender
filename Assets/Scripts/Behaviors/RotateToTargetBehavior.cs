using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTargetBehavior : MonoBehaviour
{
    public float maxDegreesPerSecond = 45.0f;
    public GameObject objectToRotate;    
    public GameObject target;

    void Update()
    {
        if (target == null || objectToRotate == null)
            return;

        var targetRotation = Quaternion.LookRotation(objectToRotate.transform.position - transform.position);
        objectToRotate.transform.rotation = Quaternion.RotateTowards(objectToRotate.transform.rotation, targetRotation, 45.0f);
    }
}
