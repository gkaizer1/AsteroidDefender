using System;
using UnityEngine;

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