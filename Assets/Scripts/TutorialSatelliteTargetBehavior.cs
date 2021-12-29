using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialSatelliteTargetBehavior : MonoBehaviour
{
    public UnityEvent onSatelliteEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gameObj = collision.gameObject;
        if (gameObj.GetComponentInChildren<SatelliteBehavior>() == null)
            return;

        onSatelliteEntered?.Invoke();
    }
}
