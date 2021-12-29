using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public GameObject LaserStart;
    public GameObject LaserEnd;
    public LineRenderer lineRenderer;

    public float LaserSpeed = 150.0f;

    Transform _target;
    Tweener _laserTween = null;

    public event System.Action OnTargetReached;
    public event System.Action OnFinishedFiring;

    bool _hasIntercept = false;
    Vector2 _interceptPoint = Vector2.zero;
    bool _firing = false;
    bool _finishingFiring = false;
    Transform _nextTarget;

    // Update is called once per frame
    void Update()
    {
        if (!_firing)
            return;
        
        if (_target == null)
        {
            if (_laserTween != null && _laserTween.IsPlaying())
            {
                _laserTween.Kill();
            }

            // Go to stop firing state
            StopFiring();
        } 
            
        // Update end position if we are not animating to the end
        if (_target != null && _laserTween == null)
        {
            Vector3 hitPoint = _target.transform.position;
            if (_hasIntercept)
            {
                hitPoint = new Vector3(_interceptPoint.x, _interceptPoint.y, 0f);
            }

            lineRenderer.SetPosition(1, hitPoint);
            LaserEnd.transform.position = hitPoint;
        }

        // Keep turrent pointing at target
        if (!_finishingFiring)
        {
            lineRenderer.SetPosition(0, transform.position);
            LaserStart.transform.position = transform.position;
        }
          
    }
    void FinishFiring()
    {
        _firing = false;
        _finishingFiring = false;
        lineRenderer.enabled = false;
        LaserStart.SetActive(false);
        LaserEnd.SetActive(false);
        OnFinishedFiring?.Invoke();

        // Switch to next target if needed
        if (_nextTarget != null)
            Fire(_nextTarget);
    }

    public void StopFiring(System.Action onCompleted = null)
    {
        if (!_firing)
            return;

        if (_finishingFiring)
            return;

        _finishingFiring = true;

        if (_nextTarget != null)
            _nextTarget = null;

        Vector3 laserDelta = lineRenderer.GetPosition(0) - lineRenderer.GetPosition(1);
        float laserReachTime = laserDelta.magnitude / LaserSpeed;

        if (laserReachTime < 0.1f)
        {
            onCompleted?.Invoke();
            FinishFiring();
        }
        
        // Show start/end effects
        LaserStart.SetActive(true);
        LaserEnd.SetActive(true);

        // Tween laser back to end - then return to idle state
        DOTween.To(
            () => lineRenderer.GetPosition(0),
            x =>
            {
                LaserStart.transform.position = x;
                lineRenderer.SetPosition(0, x);
            },
            lineRenderer.GetPosition(1),
            laserReachTime).OnComplete(() =>
            {
                onCompleted?.Invoke();
                FinishFiring();
            });      
    }


    public void Fire(Transform target)
    {
        if(_firing)
        {
            StopFiring();
            _nextTarget = target;
        }

        _nextTarget = null;
        _target = target;
        _firing = true;

        LaserStart.SetActive(true);

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new List<Vector3>()
        {
           this.transform.position,
           this.transform.position
        }.ToArray());

        float laserReachTime = Vector2.Distance(transform.position, _target.position) / LaserSpeed;

        Vector3 laserTargetPosition = _target.transform.position;
        var targetRigidBody = _target.GetComponent<Rigidbody2D>();
        if (targetRigidBody != null)
        {
            // Account for target position
            var targetTravelDistance = (targetRigidBody.velocity) * laserReachTime;

            // Account for additional distance traveled due to acceleration
            var astroid = _target.GetComponent<Astroid>();
            if (astroid)
            {
                var acceleration = astroid.settings.Acceleration;
                // 1/2 * a * t2 == distace
                targetTravelDistance += 0.5f * (acceleration * targetRigidBody.velocity.normalized) * (laserReachTime * laserReachTime);
            }

            laserTargetPosition = laserTargetPosition + new Vector3(targetTravelDistance.x, targetTravelDistance.y, _target.transform.position.z);
        }

        // Start laser destination
        _laserTween = DOTween.To(
            () => lineRenderer.GetPosition(1),
            x => {
                lineRenderer.SetPosition(1, x);
            },
            laserTargetPosition,
            laserReachTime).OnComplete(() =>
            {
                _laserTween = null;

                LaserEnd.SetActive(true);
                LaserEnd.transform.position = _target.transform.position;

                OnTargetReached?.Invoke();
            }).SetRecyclable(true);

    }
}
