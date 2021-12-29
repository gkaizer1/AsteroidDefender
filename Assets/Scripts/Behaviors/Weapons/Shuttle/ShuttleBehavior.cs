using Doozy.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShuttleBehavior : MonoBehaviour, IStateMachine
{
    public event System.Action OnShuttleDoorOpenedEvent;
    public event System.Action OnShuttleDoorClosedEvent;

    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public IState<ShuttleBehavior> _state = null;
    public string CurrentState => _state?.Name;

    [HideInInspector]
    public GameObject spacePortOfOrigin = null;

    [Header("Children")]
    public GameObject booster = null;
    public TargetIndicatorBehavior _taretIndicator = null;

    [HideInInspector]
    public Rigidbody2D rigidBody;

    public event Action OnDestroyEvent;

    public static List<GameObject> Shuttles = new List<GameObject>();

    public void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        Shuttles.Add(this.gameObject);
    }

    public void StartMission(IShuttleMission mission)
    {
        _state = mission.StartMission(this);
        this.GetComponent<SelectableBehavior>().IsSelectable = true;
    }

    private void OnDestroy()
    {
        Shuttles.Remove(this.gameObject);
        OnDestroyEvent?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if(_state != null)
            _state = _state.Update();
    }

    public FlyToTargetState<ShuttleBehavior> CreateReturnBackToBaseState()
    {
        Vector3 initialSize = this.transform.localScale;

        return new FlyToTargetState<ShuttleBehavior>(this, this.spacePortOfOrigin.transform)
        {
            decelerateHalfWay = false,
            maxRotationPerSecond = 360.0f,
        }
        .OnDestinationReached(() => new ShuttleLandingState(this))
        .OnDistanceChanged(distance =>
        {
            if (distance < 1.0f)
                this.transform.localScale = initialSize * Mathf.Clamp(distance / 1.0f, 0.5f, 1.0f);
        });
    }

    public void OnShuttleDoorOpened()
    {
        OnShuttleDoorOpenedEvent?.Invoke();
    }

    public void OnShuttleDoorClosed()
    {
        OnShuttleDoorClosedEvent?.Invoke();
    }
}
