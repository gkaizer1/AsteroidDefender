using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaypointBehavior : MonoBehaviour, IClickHander
{
    public float ROTATION_SPEED = 20.0f;

    public float SIZE_MULTIPLIER = 1.0f;

    public float SIZING_MULTIPLIER = 1.0f;

    public Vector3 OriginalScale;

    public GameObject Parent;

    public void SetParent(GameObject parent)
    {
        Parent = parent;
    }

    public void OnClicked()
    {
    }

    public void OnDestroy()
    {
        EventManager.InvokeOnObjectDestroyed(this.gameObject);
    }
}
