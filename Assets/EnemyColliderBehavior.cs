using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyColliderBehavior : MonoBehaviour
{
    public UnityEvent<GameObject> OnSatelliteDetected;
    public UnityEvent OnObstacleCleared;

    private GameObject _currentTarget = null;
    public LineRenderer lineRenderer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hitObject = collision.gameObject;

        // Only check for satellites
        if (hitObject.GetComponent<SatelliteBehavior>() is null)
            return;

        // Hit the same object as last time, nothing to do
        // This is to prevent duplicate inform
        if (hitObject == _currentTarget)
            return;

        // Do nothing if we already have a target
        if (_currentTarget != null)
            return;

        _currentTarget = hitObject;
        OnSatelliteDetected?.Invoke(hitObject);
    }

    float _timeDelta = 0.3f;
    public void Update()
    {
        if (_currentTarget != null)            
        {
            _timeDelta -= Time.deltaTime;
            if(_timeDelta < 0)
            {
                _timeDelta = 0.3f;
                var localPostion = this.transform.TransformPoint(Vector3.zero);
                var targetPosition = _currentTarget.transform.TransformPoint(Vector3.zero);

                lineRenderer.useWorldSpace = true;
                lineRenderer.enabled = true;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new Vector3[] {
                    localPostion,
                    targetPosition
                });
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var hitObject = collision.gameObject;

        // We have to use a bool since _previousHit could have
        // been destroyed and thus is null
        if (_currentTarget == hitObject)
        {
            lineRenderer.enabled = false;
            _currentTarget = null;
            OnObstacleCleared?.Invoke();
        }
    }
}
