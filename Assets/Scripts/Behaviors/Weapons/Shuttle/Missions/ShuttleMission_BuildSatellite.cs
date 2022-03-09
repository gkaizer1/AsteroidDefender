using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BuildUpgradeInfo
{
    public float timeToBuild = 2.0f;

    private bool _completed = false;
    public bool completed
    {
        get
        {
            return _completed;
        }
        set
        {
            if (value == _completed)
                return;
            _completed = value;
            if (_completed)
                OnBuildCompleted?.Invoke();
        }
    }

    public Action OnBuildCompleted = null;
}

public class ShuttleUpgradingSatelliteState : IState<ShuttleBehavior>
{
    public Rigidbody2D rigidBody;

    bool orbiting = false;
    Animator _animator;
    BuildUpgradeInfo _upgradeInfo;

    public ShuttleUpgradingSatelliteState(ShuttleBehavior parent, BuildUpgradeInfo upgradeInfo) : base(parent, false)
    {
        _upgradeInfo = upgradeInfo;
        Init();
    }

    public override string Name
    {
        get
        {
            return "Upgrading Satellite Resources";
        }
    }

    public override void Init()
    {
        base.Init();
        rigidBody = Parent.GetComponent<Rigidbody2D>();
        rigidBody.velocity = Vector2.zero;
        orbiting = false;

        _animator = Parent.GetComponent<Animator>();
        Parent.OnShuttleDoorOpenedEvent += OnShuttleDoorOpened;
    }

    private void OnShuttleDoorOpened()
    {
        Parent.OnShuttleDoorOpenedEvent -= OnShuttleDoorOpened;
        Parent.OnShuttleDoorClosedEvent += OnShuttleDoorClosed;
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

        if (orbiting)
        {
            if (!_upgradeInfo.completed)
            {
                _upgradeInfo.timeToBuild -= Time.deltaTime;
                if (_upgradeInfo.timeToBuild <= 0f)
                {
                    _upgradeInfo.completed = true;
                    ReturnHome();
                }
            }
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


public class ShuttleMission_UpgradeSatellite : IShuttleMission
{
    public bool MissionAccepted { get; set; } = false;

    GameObject _target = null;
    BuildUpgradeInfo _upgradeInfo = null;
    public ShuttleMission_UpgradeSatellite(GameObject target, BuildUpgradeInfo upgradeInfo)
    {
        _target = target;
        _upgradeInfo = upgradeInfo;
    }

    public IState<ShuttleBehavior> StartMission(ShuttleBehavior shuttle)
    {
        MissionAccepted = true;

        var nextState = new FlyToTargetState<ShuttleBehavior>(shuttle, _target.transform)
        {
            StopDistance = 0.5f,
            Acceleration = 0.5f,
            maxRotationPerSecond = 90.0f,
        }
        .OnDestinationReached(() => new ShuttleUpgradingSatelliteState(shuttle, _upgradeInfo));

        shuttle.target = _target;

        return new ShuttleLaunchingState(shuttle, nextState);
    }
}

public class ShuttleMission_BuildSatellite : IShuttleMission
{
    GameObject _target = null;
    ConstructableBehavior _constructableBehavior = null;
    public bool MissionAccepted { get; set; } = false;

    public ShuttleMission_BuildSatellite(GameObject target, ConstructableBehavior underConstructionTile)
    {
        _constructableBehavior = underConstructionTile;
        _target = target;
    }

    public virtual IState<ShuttleBehavior> StartMission(ShuttleBehavior shuttle)
    {
        MissionAccepted = true;

        var nextState = new FlyToTargetState<ShuttleBehavior>(shuttle, _target.transform)
        {
            StopDistance = 0.5f,
            Acceleration = 0.5f,
            maxRotationPerSecond = 90.0f,
        }
        .OnDestinationReached(() => new ShuttleBuildingSatelliteState(shuttle, _constructableBehavior));

        shuttle.target = _target;

        return new ShuttleLaunchingState(shuttle, nextState);
    }
}