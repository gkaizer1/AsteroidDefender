using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    public DebugOverlay debugOverlay;

    // Start is called before the first frame update
    void Start()
    {
        debugOverlay.Create();
    }
}
