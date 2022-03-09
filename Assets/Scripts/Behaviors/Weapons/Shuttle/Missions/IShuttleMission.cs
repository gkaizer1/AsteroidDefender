using System;

public interface IShuttleMission
{
    bool MissionAccepted { get; set; }
    IState<ShuttleBehavior> StartMission(ShuttleBehavior shuttle);
}
