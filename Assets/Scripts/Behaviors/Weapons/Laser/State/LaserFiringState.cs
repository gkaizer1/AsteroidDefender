using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LaserFiringState : IState<LaserBehavior>
{
    Tweener _laserTween = null;
    AutoAimBehavior _autoAimBehavior = null;
    Coroutine _damageCoroutine = null;

    float damagePerSecond = 10.0f;

    public GameObject _target;

    bool _hasIntercept = false;
    Vector2 _interceptPoint = Vector2.zero;

    public LaserFiringState(LaserBehavior obj) : base(obj)
    {
    }

    public override string Name
    {
        get
        {
            return "Firing";
        }
    }

    public override void Init()
    {
        _laserTween = null;
        _damageCoroutine = null;
        
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();

        damagePerSecond = Parent.damagePerSecond;

        Parent.LaserStart.SetActive(true);
        Parent.FiringLight.SetActive(true);

        Parent.lineRenderer.enabled = true;
        Parent.lineRenderer.positionCount = 2;
        Parent.lineRenderer.SetPositions(new List<Vector3>()
        {
           Parent.laserTip.position,
           Parent.laserTip.position
        }.ToArray());

        _target = _autoAimBehavior.currentTarget;

        Vector2 delta = Parent.laserTopAnchor.transform.position - _target.transform.position;

        float laserSpeed = 150.0f;
        float laserReachTime = delta.magnitude / laserSpeed;

        Vector3 laserTargetPosition = _target.transform.position;
        var targetRigidBody = _target.GetComponent<Rigidbody2D>();
        if(targetRigidBody != null)
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
            () => Parent.lineRenderer.GetPosition(1),
            x => {
                Parent.lineRenderer.SetPosition(1, x);
            },
            laserTargetPosition,
            laserReachTime).OnComplete(() =>
            {
                _laserTween = null;

                Parent.LaserEnd.SetActive(true);
                Parent.LaserEnd.transform.position = _target.transform.position;

                _damageCoroutine = Parent.StartCoroutine(DamageTargetCoRoutine());
            }).SetRecyclable(true);
    }

    IEnumerator DamageTargetCoRoutine()
    {
        float updateTime = 0.1f;
        while (true)
        {
            if (_target == null)
                break;

            var hitObject = _target;

            // Check if anything entered into the ray
            {
                int astroid_layer = 1 << LayerMask.NameToLayer("enemies");

                // Ray cast tip up to infinity
                var astroids = Physics2D.Raycast(
                    Parent.laserTip.transform.position, 
                    Parent.laserTip.transform.up, 
                    float.MaxValue, 
                    astroid_layer);

                if (astroids.collider != null)
                {
                    hitObject = astroids.collider.gameObject;
                    _interceptPoint = astroids.point;
                    _hasIntercept = true;
                }
                else
                {
                    _hasIntercept = false;
                }
            }

            if (hitObject != null)
                hitObject.GetComponent<HealthBehavior>().Health -= (damagePerSecond * updateTime);

            yield return new WaitForSeconds(updateTime);
        }
    }

    public override IState<LaserBehavior> Update()
    {
        // No target or out of power - transition to stop firing
        if (_target == null || _autoAimBehavior.currentTarget != _target)
        {
            if (_damageCoroutine != null)
            {
                Parent.StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }

            if(_laserTween != null && _laserTween.IsPlaying())
            {
                _laserTween.Kill();
            }

            // Go to stop firing state
            return new LaserStopFiringState(Parent);
        }
        
        // Update end position if we are not animating to the end
        if (_laserTween == null)
        {
            Vector3 hitPoint = _target.transform.position;
            if (_hasIntercept)
            {
                hitPoint = new Vector3(_interceptPoint.x, _interceptPoint.y, 0f);
            }

            Parent.lineRenderer.SetPosition(1, hitPoint);
            Parent.LaserEnd.transform.position = hitPoint;
        }

        Vector2 delta = _target.transform.position - Parent.laserTopAnchor.transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, delta);

        // Keep turrent pointing at target
        Parent.laserTopAnchor.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Parent.lineRenderer.SetPosition(0, Parent.laserTip.transform.position);
        Parent.LaserStart.transform.position = Parent.laserTip.transform.position;

        return this;
    }
}