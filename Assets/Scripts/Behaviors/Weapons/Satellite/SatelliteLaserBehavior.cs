using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteLaserStopFiringState : IState<SatelliteLaserBehavior>
{
    public override string Name
    {
        get
        {
            return "Cooling Off";
        }
    }

    public SatelliteLaserStopFiringState(SatelliteLaserBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        Vector3 laserDelta = Parent.lineRenderer.GetPosition(0) - Parent.lineRenderer.GetPosition(1);
        float laserSpeed = 150.0f;
        float laserReachTime = laserDelta.magnitude / laserSpeed;

        if (laserReachTime > float.Epsilon)
        {
            // Show start/end effects
            Parent.LaserStart.SetActive(true);
            Parent.LaserEnd.SetActive(true);

            // Tween laser back to end - then return to idle state
            DOTween.To(
                () => Parent.lineRenderer.GetPosition(0),
                x =>
                {
                    Parent.LaserStart.transform.position = x;
                    Parent.lineRenderer.SetPosition(0, x);
                },
                Parent.lineRenderer.GetPosition(1),
                laserReachTime).OnComplete(() =>
                {
                    FinishFiring();
                });
        }
        else
        {
            FinishFiring();
        }
    }

    void FinishFiring()
    {
        Parent.LaserStart.SetActive(false);
        Parent.LaserEnd.SetActive(false);
        _nextState = new SatelliteLaserRotateToTargetState(Parent);
    }
}

public class SatelliteLaserFiringState : IState<SatelliteLaserBehavior>
{
    Tweener _laserTween = null;
    Coroutine _damageCoroutine = null;
    GameObject _target = null;
    bool _hasIntercept = false;
    Vector2 _interceptPoint = Vector2.zero;

    AutoAimBehavior _autoAimBehavior = null;

    public SatelliteLaserFiringState(SatelliteLaserBehavior obj) : base(obj)
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

        _target = Parent.GetComponent<AutoAimBehavior>().currentTarget;
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();

        Parent.LaserStart.SetActive(true);

        Parent.lineRenderer.enabled = true;
        Parent.lineRenderer.positionCount = 2;
        Parent.lineRenderer.SetPositions(new List<Vector3>()
        {
           Parent.laserFirePoint.transform.position,
           Parent.laserFirePoint.transform.position,
        }.ToArray());

        Vector2 delta = Parent.laserHeader.transform.position - _target.transform.position;

        float laserSpeed = 150.0f;
        float laserReachTime = delta.magnitude / laserSpeed;

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
        while (_target != null)
        {
            if (_target == null)
                break;

            var hitObject = _target;

            // Check if anything entered into the ray
            {
                int astroid_layer = 1 << LayerMask.NameToLayer("enemies");
                var astroids = Physics2D.Raycast(Parent.laserFirePoint.transform.position, Parent.laserFirePoint.transform.up, float.MaxValue, astroid_layer);
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
                hitObject.GetComponent<HealthBehavior>().Health -= (Parent.DamagePerSecond * updateTime);

            yield return new WaitForSeconds(updateTime);
        }
    }

    public override IState<SatelliteLaserBehavior> Update()
    {
        // No target or out of power - transition to stop firing
        if (_target == null || _autoAimBehavior.currentTarget != _target)
        {
            if (_damageCoroutine != null)
            {
                Parent.StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }

            if (_laserTween != null && _laserTween.IsPlaying())
            {
                _laserTween.Kill();
            }

            // Go to stop firing state
            return new SatelliteLaserStopFiringState(Parent);
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

        Vector2 delta = _target.transform.position - Parent.laserHeader.transform.position;
        float angle = Vector2.SignedAngle(Vector2.down, delta);

        // Keep turrent pointing at target
        Parent.laserHeader.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Parent.lineRenderer.SetPosition(0, Parent.laserFirePoint.transform.position);
        Parent.LaserStart.transform.position = Parent.laserFirePoint.transform.position;

        return this;
    }
}

public class SatelliteLaserRotateToTargetState : IState<SatelliteLaserBehavior>
{
    public override string Name => "Acquiring Target";

    AutoAimBehavior _autoAimBehavior = null;
    float coolDownTimer = 2.0f;
    float coolDownTime = 2.0f;

    public SatelliteLaserRotateToTargetState(SatelliteLaserBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
        Parent.LaserStart.SetActive(false);
        Parent.LaserEnd.SetActive(false);
    }

    public override IState<SatelliteLaserBehavior> Update()
    {
        var target = _autoAimBehavior.currentTarget;
        if (target == null)
        {
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer < 0)
                return new SatelliteLaserIdleState(Parent);
            else
                return this;
        }

        // Reset the cooldown
        coolDownTimer = coolDownTime;

        Vector2 delta = target.transform.position - Parent.laserHeader.transform.position;
        float angle = Vector2.SignedAngle(Vector2.down, delta);
        float angleDelta = Mathf.DeltaAngle(Parent.laserHeader.transform.rotation.eulerAngles.z, angle);

        float anglePerStep = Parent.RotationAnglePerSecond * Time.deltaTime;

        float rotation = Mathf.Clamp(angleDelta, -anglePerStep, anglePerStep);
        Parent.laserHeader.transform.rotation = Quaternion.Euler(
            0f, 
            0f, 
            Parent.laserHeader.transform.rotation.eulerAngles.z + rotation);

        if (Mathf.Abs(angleDelta) < 0.2f)
        {
            return new SatelliteLaserFiringState(Parent);
        }

        return this;
    }
}

public class SatelliteLaserIdleState : IState<SatelliteLaserBehavior>
{
    public override string Name => "Idle";

    AutoAimBehavior _autoAimBehavior = null;
    RotatorBehavior _rotator = null;

    public SatelliteLaserIdleState(SatelliteLaserBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
        _rotator = Parent.laserHeader.GetComponent<RotatorBehavior>();
        _rotator.enabled = true;

        Parent.LaserStart.SetActive(false);
        Parent.LaserEnd.SetActive(false);
    }

    public override IState<SatelliteLaserBehavior> Update()
    {
        if (_autoAimBehavior.currentTarget != null)
        {
            _rotator.enabled = false;
            return new SatelliteLaserRotateToTargetState(Parent);
        }

        return this;
    }
}

public class SatelliteLaserBehavior : MonoBehaviour
{
    [Header("Children")]
    public GameObject laserHeader;
    public GameObject laserFirePoint;

    public LineRenderer lineRenderer;
    public GameObject LaserStart;
    public GameObject LaserEnd;

    [Header("Settings")]
    public float RotationAnglePerSecond = 90.0f;
    public float DamagePerSecond = 10.0f;

    IState<SatelliteLaserBehavior> _state;

    void Awake()
    {
        //_state = new SatelliteLaserIdleState(this);
    }

    // Update is called once per frame
    void Update()
    {
        //_state = _state.Update();
    }
}
