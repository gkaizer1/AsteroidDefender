using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FlyToTargetState<T> : IState<T>
     where T : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public float StopDistance = 0.1f;

    public float Acceleration = 1.0f;
    public float maxRotationPerSecond = 180.0f;
    public float maxSpeed = 4.0f;
    public float _initialDistance = 1.0f;
    public bool decelerateHalfWay = true;
    public bool stopAtTarget = true;

    Transform _destination = null;

    float _speedAtDeceleration = 0.0f;
    float _decelerationDistance = 0.0f;
    Vector3 _initialSize = Vector3.one;

    public delegate void OnDistanceChangedCallback(float distance);
    private OnDistanceChangedCallback _onDistanceChanged = null;

    public delegate IState<T> OnDestinationReachedCallback();
    public OnDestinationReachedCallback _onDestinationReached;
    public OnDestinationReachedCallback _onDestinationDestroyed;

    public Vector3 _destinationPoint;
    bool _hasDestinationObject = false;

    public FlyToTargetState(T parent, Transform destination) : base(parent, false)
    {
        _destination = destination;
        _hasDestinationObject = true;
        _destinationPoint = _destination.position;
        Init();
    }
    public FlyToTargetState(T parent, Vector3 destination) : base(parent, false)
    {
        _destinationPoint = destination;
        Init();
    }

    public FlyToTargetState<T> OnDestinationReached(OnDestinationReachedCallback callback)
    {
        _onDestinationReached = callback;
        return this;
    }

    public FlyToTargetState<T> OnDestinationDestroyed(OnDestinationReachedCallback callback)
    {
        _onDestinationDestroyed = callback;
        return this;
    }

    public FlyToTargetState<T> OnDistanceChanged(OnDistanceChangedCallback callBack)
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
        var flyingBehavior = Parent.GetComponent<FlyingBehavior>();
        if (flyingBehavior == null)
        {
            Acceleration = 0.5f;
            maxRotationPerSecond = 45.0f;
            maxSpeed = 2.0f;
        }
        else
        {
            Acceleration = flyingBehavior.Acceleration;
            maxRotationPerSecond = flyingBehavior.MaxRotationPerSecond;
            maxSpeed = flyingBehavior.MaxSpeed;
            decelerateHalfWay = flyingBehavior.StopHalfwayThrough;
            stopAtTarget = flyingBehavior.StopAtTarget;
        }

        _initialDistance = 1.0f;

        rigidBody = Parent.GetComponent<Rigidbody2D>();

        Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
        Vector2 destination = new Vector2(_destinationPoint.x, _destinationPoint.y);
        _initialDistance = Vector2.Distance(currentPostion, destination);
        _decelerationDistance = (_initialDistance / 3.0f);
        _initialSize = Parent.transform.localScale;
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

        if (_onDestinationReached == null)
            throw new Exception("Forgot to set [OnDestinationReached] callback for FlyToTargetState");

        // We had a destination - but now it's gone :(
        if (_destination == null && _hasDestinationObject)
        {
            return _onDestinationDestroyed();
        }

        // Update destination point if we have a target rather than point
        if (_destination != null)
        {
            _destinationPoint = _destination.position;
        }

        Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
        Vector2 destination = new Vector2(_destinationPoint.x, _destinationPoint.y);
        Vector2 delta = destination - currentPostion;

        float deltaAngle = Vector2.SignedAngle(Parent.transform.up, delta);
        this.rigidBody.rotation += Mathf.Clamp(deltaAngle, -maxRotationPerSecond * Time.deltaTime, maxRotationPerSecond * Time.deltaTime);

        this.rigidBody.velocity = Utils.rotateVector2(new Vector2(0.0f, this.rigidBody.velocity.magnitude), this.rigidBody.rotation);

        if (_onDistanceChanged != null)
            _onDistanceChanged(delta.magnitude);

        if (delta.magnitude < StopDistance)
        {
            if(stopAtTarget)
                this.rigidBody.velocity = Vector2.zero;

            if(_onDestinationReached != null)
            {
                return _onDestinationReached();
            }
            return this;
        }
        else
        {
            if (decelerateHalfWay)
            {
                if (delta.magnitude > _decelerationDistance)
                    UpdateAcceleration(Acceleration);
                else
                {
                    // Save speed when we started to decelerate
                    if (_speedAtDeceleration == 0.0f)
                    {
                        _speedAtDeceleration = rigidBody.velocity.magnitude;
                    }
                    rigidBody.velocity = rigidBody.velocity.normalized * _speedAtDeceleration * Mathf.Clamp((0.3f + (0.7f) * (delta.magnitude / _decelerationDistance)), 0.3f, _speedAtDeceleration);
                }
            }
            else
                UpdateAcceleration(Acceleration);
        }

        return this;
    }
}