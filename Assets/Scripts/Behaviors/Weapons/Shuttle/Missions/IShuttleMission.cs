using System;

public interface IShuttleMission
{
    bool MissionAccepted { get; }
    IState<ShuttleBehavior> StartMission(ShuttleBehavior shuttle);
}
