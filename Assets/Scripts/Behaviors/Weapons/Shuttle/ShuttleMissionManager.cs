using Doozy.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuttleMissionManager
{
    public List<IShuttleMission> missions = new List<IShuttleMission>();
    public static event Action<IShuttleMission> OnMissionScheduled;

    private static ShuttleMissionManager _instance;
    public static ShuttleMissionManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ShuttleMissionManager();

            return _instance;
        }
    }

    public void ScheduleMission(IShuttleMission mission)
    {
        missions.Add(mission);
        OnMissionScheduled?.Invoke(mission);
    }

    public void StartMission(ShuttleBehavior shuttle, IShuttleMission mission)
    {
        shuttle.StartMission(mission);    
        missions.Remove(mission);
    }
}
