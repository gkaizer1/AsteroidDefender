using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidsContainer : MonoBehaviour
{
    public static GameObject Instance;

    private void Awake()
    {
        Instance = this.gameObject;
    }
}
