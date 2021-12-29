using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShuttleCollectingState : IState<ShuttleBehavior>
{
    public Rigidbody2D rigidBody;

    float maxRotationPerSecond = 180.0f;
    bool orbiting = false;
    Animator _animator;
    Vector3 _orbitPoint = Vector3.zero;

    // How far has the resource been pulled
    float _pullPrecent = 0.0f;
    float _pullTime = 5.0f;
    Vector3 _targetInitialSize = Vector3.one;
    Vector3 _initialSize = Vector3.one;
    bool _doorOpen = false;

    public ShuttleCollectingState(ShuttleBehavior parent) : base(parent)
    {

    }

    public override string Name
    {
        get
        {
            return "Collecting Resources";
        }
    }

    public override void Init()
    {
        rigidBody = Parent.GetComponent<Rigidbody2D>();
        _animator = Parent.GetComponent<Animator>();

        rigidBody.velocity = Vector2.zero;
        _pullPrecent = 0.0f;
        _pullTime = 5.0f;
        _orbitPoint = Vector3.zero;
        orbiting = false;
        _targetInitialSize = Parent.target.transform.localScale;
        _initialSize = Parent.transform.localScale;
        Parent.OnShuttleDoorOpenedEvent += OnShuttleDoorOpened;
    }

    private void OnShuttleDoorOpened()
    {
        Parent.OnShuttleDoorOpenedEvent -= OnShuttleDoorOpened;
        Parent.OnShuttleDoorClosedEvent += OnShuttleDoorClosed;
        _doorOpen = true;
    }

    private void OnShuttleDoorClosed()
    {
        Parent.OnShuttleDoorClosedEvent += OnShuttleDoorClosed;
        _doorOpen = false;

        var onLandedState = new ShuttleLandingState(Parent).OnLanded(() =>
        {
            if(Parent.target != null && Parent.target.GetComponent<MineralBehavior>() != null)
                Parent.target.GetComponent<MineralBehavior>().AddToResource();

            GameObject.Destroy(Parent.target);
        });
        _nextState = Parent.CreateReturnBackToBaseState().OnDestinationReached(() => onLandedState);
    }

    public override IState<ShuttleBehavior> Update()
    {
        if (Parent.target == null)
        {
            return Parent.CreateReturnBackToBaseState();
        }

        Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
        Vector2 targetPosition = new Vector2(Parent.target.transform.position.x, Parent.target.transform.position.y);
        Vector2 delta = targetPosition - currentPostion;

        if (orbiting)
        {
            Parent.transform.RotateAround(_orbitPoint, -Parent.transform.transform.forward, 30.0f * Time.deltaTime);

            // Move the object in the direction needed
            if (delta.magnitude > 0.01f)
            {
                Vector2 orbitDelta = currentPostion - new Vector2(_orbitPoint.x, _orbitPoint.y);

                if (_doorOpen)
                {
                    Vector3 pullDelta = orbitDelta * _pullPrecent;
                    _pullPrecent = Mathf.Clamp01(_pullPrecent + (1.0f / _pullTime) * Time.deltaTime);
                    Parent.target.transform.position = _orbitPoint + new Vector3(pullDelta.x, pullDelta.y);
                    Parent.target.transform.localScale = _targetInitialSize * Mathf.Clamp(1.0f - _pullPrecent, 0.75f, 1.0f);
                    if (_pullPrecent >= 1.0f)
                    {
                        Parent.target.transform.parent = Parent.transform;
                        _doorOpen = false;
                        _animator.SetTrigger("CLOSE_DOOR");
                        return _nextState;
                    }
                }
            }

            return this;
        }

        Vector2 perp = Vector2.Perpendicular(delta);
        float deltaAngle = Vector2.SignedAngle(Parent.transform.up, perp);
        this.rigidBody.rotation += Mathf.Clamp(deltaAngle, -maxRotationPerSecond * Time.deltaTime, maxRotationPerSecond * Time.deltaTime);

        if (deltaAngle < 0.02f)
        {
            orbiting = true;
            _animator.SetTrigger("ARRIVED_AT_DESTINATION");
            _orbitPoint = targetPosition;
        }

        return this;
    }
}
