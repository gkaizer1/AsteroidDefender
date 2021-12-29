using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToCanvasBehavior : MonoBehaviour
{
    public string canvasTag;

    void Awake()
    {
        var objs = GameObject.FindGameObjectsWithTag(canvasTag);
        if (objs.Length <= 0)
            return;

        var canvas = GameObject.FindGameObjectsWithTag(canvasTag)[0] as GameObject;
        this.transform.SetParent(canvas.transform, true);
    }
}
