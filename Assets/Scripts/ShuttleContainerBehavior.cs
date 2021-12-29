using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuttleContainerBehavior : MonoBehaviour
{
    public static ShuttleContainerBehavior Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
}
