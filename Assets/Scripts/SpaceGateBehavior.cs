using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class SpaceGateBehavior : MonoBehaviour
{
    public GameObject portal;
    public UnityEvent OnGateSpawned;
    public UnityEvent OnGateClosed;
    
    public float secondsToOpen = 3.0f;

    private void Start()
    {
        // hide the portal
        portal.SetActive(false);
    }

    public void OpenSpawnGate()
    {
        portal.SetActive(true);
        portal.transform.localScale = Vector3.zero;
        portal.transform.DOScale(Vector3.one, secondsToOpen)
            .OnComplete(() => 
            {
                OnGateSpawned?.Invoke();
            });
    }

    public void CloseSpawnGate()
    {
        portal.transform.DOScale(Vector3.zero, secondsToOpen)
            .OnComplete(() => OnGateClosed?.Invoke());
    }

    public void ZoomToGate()
    {
        ZoomBehavior.Instance.ZoomToPoint(this.transform.position, 3.0f);
    }
}
