using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaserPanelBehavior : MonoBehaviour, ISelectionPanel
{
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public LaserBehavior laserBehavior;

    public void SetTarget(GameObject gameObject)
    {
        target = gameObject;
        laserBehavior = target.GetComponent<LaserBehavior>();
    }
}
