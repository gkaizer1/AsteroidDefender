using System;
using UnityEngine;

public class ShuttleMission_BuildSatellite : IShuttleMission
{
    GameObject _target = null;
    TileUnderConstructionBehavior _underConstructionTile = null;
    public bool MissionAccepted { get; set; } = false;

    public ShuttleMission_BuildSatellite(GameObject target, TileUnderConstructionBehavior underConstructionTile)
    {
        _underConstructionTile = underConstructionTile;
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
        .OnDestinationReached(() => new ShuttleBuildingSatelliteState(shuttle, _underConstructionTile));

        shuttle.target = _target;

        return new ShuttleLaunchingState(shuttle, nextState);
    }
}