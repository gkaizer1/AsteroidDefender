using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointRotatorSingleton : MonoBehaviour
{
    public static float ROTATION_SPEED = 40.0f;

    public static float SIZE_MULTIPLIER = 1.0f;

    public static float SIZING_DIRECTION = 1.0f;

    public static Quaternion CURRENT_ROTATION;

    public static readonly float MAX_SIZE = 2.0f;
    public static readonly float MIN_SIZE = 0.5f;

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(Vector3.forward * ROTATION_SPEED * Time.deltaTime);

        SIZE_MULTIPLIER += SIZING_DIRECTION * 0.2f * Time.deltaTime;
        if (SIZE_MULTIPLIER > 2.0f)
        {
            SIZING_DIRECTION = -1.0f;
        }
        if (SIZE_MULTIPLIER < 0.8f)
        {
            SIZING_DIRECTION = 1.0f;
        }

        CURRENT_ROTATION = this.transform.rotation;
    }
}
