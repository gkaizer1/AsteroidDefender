using Doozy.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShuttleMission_CollectResoure : IShuttleMission
{
    GameObject _target;
    public bool MissionAccepted { get; set; } = false;

    public ShuttleMission_CollectResoure(GameObject target)
    {
        _target = target;
    }

    public virtual IState<ShuttleBehavior> StartMission(ShuttleBehavior shuttle)
    {
        // Emit a message that a shuttle is going to collect this resource
        // This is to stop the resources 'death timer'
        GameEventMessage.SendEvent("SHUTTLE.COLLECT_MISSION", shuttle.gameObject, _target);

        MissionAccepted = true;
        var nextState = new FlyToTargetState<ShuttleBehavior>(shuttle,_target.transform)
        {

            StopDistance = 1.0f,
            Acceleration = 0.5f,
        }
        .OnDestinationReached(() => new ShuttleCollectingState(shuttle));
        shuttle.target = _target;

        return new ShuttleLaunchingState(shuttle, nextState);
    }
}
