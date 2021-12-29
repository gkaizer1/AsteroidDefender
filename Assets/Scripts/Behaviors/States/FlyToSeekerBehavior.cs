using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Pathfinding;

public class FlyToSeekerBehavior<T> : IState<T>
     where T : MonoBehaviour
{
    public Seeker _seeker;
    public Rigidbody2D rigidBody;
    public float StopDistance = 0.1f;

    public float Acceleration = 1.0f;
    public float maxRotationPerSecond = 180.0f;
    public float maxSpeed = 4.0f;
    public float _initialDistance = 1.0f;
    public bool decelerateHalfWay = true;
    public bool stopAtTarget = true;

    Path _path = null;
    int currentPathIndex = -1;


    float _speedAtDeceleration = 0.0f;
    float _decelerationDistance = 0.0f;

    public delegate void OnDistanceChangedCallback(float distance);
    private OnDistanceChangedCallback _onDistanceChanged = null;

    public delegate IState<T> OnDestinationReachedCallback();
    public OnDestinationReachedCallback _onDestinationReached;

    public Vector2 _destinationPoint;

    public FlyToSeekerBehavior(T parent, Vector2 destination) : base(parent, false)
    {
        _destinationPoint = destination;
        Init();
    }

    public FlyToSeekerBehavior<T> OnDestinationReached(OnDestinationReachedCallback callback)
    {
        _onDestinationReached = callback;
        return this;
    }

    public FlyToSeekerBehavior<T> OnDistanceChanged(OnDistanceChangedCallback callBack)
    {
        _onDistanceChanged = callBack;
        return this;
    }

    public override string Name
    {
        get
        {
            return "Flying to target";
        }
    }

    public override void Init()
    {
        _seeker = Parent.GetComponent<Seeker>();

        var flyingBehavior = Parent.GetComponent<FlyingBehavior>();
        if (flyingBehavior == null)
            throw new Exception($"Forgot to add [FlyingBehavior] for [{Parent.name}]");


        Acceleration = flyingBehavior.Acceleration;
        maxRotationPerSecond = flyingBehavior.MaxRotationPerSecond;
        maxSpeed = flyingBehavior.MaxSpeed;
        decelerateHalfWay = flyingBehavior.StopHalfwayThrough;
        stopAtTarget = flyingBehavior.StopAtTarget;

        _initialDistance = 1.0f;

        rigidBody = Parent.GetComponent<Rigidbody2D>();

        Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
        Vector2 destination = new Vector2(_destinationPoint.x, _destinationPoint.y);
        _initialDistance = Vector2.Distance(currentPostion, destination);
        _decelerationDistance = (_initialDistance / 3.0f);

        SatelliteBehavior.OnSatelliteMoved += OnSatelliteMoved;
    }

    public void OnPathUpdated(Path path)
    {
        if (path.error)
        {
            throw new Exception("PathFinding Exception");
        }
        _path = path;
        currentPathIndex = 0;
    }

    public override void OnFirstUpdate()
    {
        base.OnFirstUpdate();
        _seeker.StartPath(Parent.transform.position, _destinationPoint, OnPathUpdated);
        if (_onDestinationReached == null)
            Parent.gameObject.UnassignedReference("Forgot to set [OnDestinationReached] callback for FlyToTargetState");
    }
    public override void Destroy()
    {
        SatelliteBehavior.OnSatelliteMoved -= OnSatelliteMoved;
        base.Destroy();
    }

    private void OnSatelliteMoved()
    {
        _seeker.StartPath(Parent.transform.position, _destinationPoint, OnPathUpdated);
    }

    void UpdateAcceleration(float Acceleration)
    {
        rigidBody.velocity += new Vector2(Parent.transform.up.x, Parent.transform.up.y) * Acceleration * Time.deltaTime;
        if (rigidBody.velocity.magnitude > maxSpeed)
            rigidBody.velocity = rigidBody.velocity.normalized * maxSpeed;
    }

    public override IState<T> Update()
    {
        base.Update();

        if (currentPathIndex < 0 || _path == null)
            return _nextState;

        if (currentPathIndex < _path.vectorPath.Count)
        {
            Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
            Vector2 nextDestinationPoint = _path.vectorPath[currentPathIndex];
            Vector2 delta = nextDestinationPoint - currentPostion;
            float deltaAngle = Vector2.SignedAngle(Parent.transform.up, delta);
            this.rigidBody.rotation += Mathf.Clamp(deltaAngle, -maxRotationPerSecond * Time.deltaTime, maxRotationPerSecond * Time.deltaTime);
            this.rigidBody.velocity = Utils.rotateVector2(new Vector2(0.0f, this.rigidBody.velocity.magnitude), this.rigidBody.rotation);

            if (delta.magnitude < 0.2f)
                currentPathIndex++;
        }

        float destinationDistance = (new Vector2(Parent.transform.position.x, Parent.transform.position.y) - this._destinationPoint).magnitude;
        if (_onDistanceChanged != null)
            _onDistanceChanged(destinationDistance);

        if (destinationDistance < StopDistance)
        {
            SatelliteBehavior.OnSatelliteMoved -= OnSatelliteMoved;
            if (stopAtTarget)
                this.rigidBody.velocity = Vector2.zero;

            if (_onDestinationReached != null)
            {
                return _onDestinationReached();
            }
            return _nextState;
        }
        else
        {
            if (decelerateHalfWay)
            {
                if (destinationDistance > _decelerationDistance)
                    UpdateAcceleration(Acceleration);
                else
                {
                    // Save speed when we started to decelerate
                    if (_speedAtDeceleration == 0.0f)
                    {
                        _speedAtDeceleration = rigidBody.velocity.magnitude;
                    }
                    rigidBody.velocity = rigidBody.velocity.normalized * _speedAtDeceleration * Mathf.Clamp((0.3f + (0.7f) * (destinationDistance / _decelerationDistance)), 0.3f, _speedAtDeceleration);
                }
            }
            else
                UpdateAcceleration(Acceleration);
        }

        return _nextState;
    }
}