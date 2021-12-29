using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsContainerBehavior : MonoBehaviour
{
    public static WeaponsContainerBehavior Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
}
