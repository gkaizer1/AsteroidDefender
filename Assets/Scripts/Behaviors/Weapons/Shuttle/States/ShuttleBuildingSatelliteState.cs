using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShuttleBuildingSatelliteState : IState<ShuttleBehavior>
{
    public Rigidbody2D rigidBody;

    bool orbiting = false;
    Animator _animator;

    ConstructableBehavior _constructableBehavior;

    public ShuttleBuildingSatelliteState(ShuttleBehavior parent, ConstructableBehavior tileUnderConstruction) : base(parent, false)
    {
        _constructableBehavior = tileUnderConstruction;
        Init();
    }

    public override string Name
    {
        get
        {
            return "Building Satellite Resources";
        }
    }

    public override void Init()
    {
        base.Init();
        rigidBody = Parent.GetComponent<Rigidbody2D>();
        rigidBody.velocity = Vector2.zero;
        orbiting = false;

        _animator = Parent.GetComponent<Animator>();
        _constructableBehavior.OnConstructionCompleted.AddListener(OnConstructionCompleted);
        Parent.OnShuttleDoorOpenedEvent += OnShuttleDoorOpened;
    }

    private void OnShuttleDoorOpened()
    {
        Parent.OnShuttleDoorOpenedEvent -= OnShuttleDoorOpened;
        Parent.OnShuttleDoorClosedEvent += OnShuttleDoorClosed;
        if (_constructableBehavior != null)
        {
            _constructableBehavior.IsBuilding = true;
        }
    }

    public void OnConstructionCompleted(GameObject newTile)
    {
        ReturnHome();
    }

    private void OnShuttleDoorClosed()
    {
        Parent.OnShuttleDoorClosedEvent -= OnShuttleDoorClosed;
        Parent.transform.parent = null;
        _nextState = Parent.CreateReturnBackToBaseState();
    }

    public void ReturnHome()
    {
        Parent.transform.parent = null;
        if (_constructableBehavior != null)
        {
            _constructableBehavior.OnConstructionCompleted.RemoveListener(OnConstructionCompleted);
        }
        _animator.SetTrigger("CLOSE_DOOR");
        Parent.OnShuttleDoorClosedEvent += OnShuttleDoorClosed;
    }

    public override void OnFirstUpdate()
    {
        Parent.transform.parent = Parent.target.transform;
    }

    public override IState<ShuttleBehavior> Update()
    {
        base.Update();

        //if (Parent.target == null)
        //{
        //    Parent.transform.parent = null;
        //    return Parent.CreateReturnBackToBaseState();
        //}

        if (orbiting)
        {
            return _nextState;
        }

        
        Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
        Vector2 targetPosition = new Vector2(Parent.target.transform.position.x, Parent.target.transform.position.y);
        Vector2 delta = targetPosition - currentPostion;

        Vector2 perp = Vector2.Perpendicular(delta);
        float deltaAngle = Vector2.SignedAngle(Parent.transform.up, perp);
        float rotation = this.Parent.transform.rotation.eulerAngles.z;
        float maxRotation = 45.0f;
        float newRotation = rotation + Mathf.Clamp(deltaAngle, -maxRotation * Time.deltaTime, maxRotation * Time.deltaTime);
        this.Parent.transform.rotation = Quaternion.Euler(0f, 0f, newRotation);

        if (deltaAngle < 0.02f)
        {
            Parent.transform.parent = null;
            _animator.SetTrigger("ARRIVED_AT_DESTINATION");
            orbiting = true;
        }
        

        return this;
    }
}
