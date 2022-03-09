using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ShuttleTileBehavior : MonoBehaviour
{
    public GameObject shuttlePrefab;

    [HideInInspector]
    public GameObject _shuttle;

    [HideInInspector]
    public bool _hasShuttle = true;

    public ParticleSystem takeoffParticle;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(co_PollMissionManager());

        ShuttleMissionManager.OnMissionScheduled += OnMissionScheduled;

        SpawnShuttle();
    }

    private void OnMissionScheduled(IShuttleMission mission)
    {
        if (mission.MissionAccepted)
            return;

        StartMission(mission);
    }

    public void SpawnShuttle()
    {
        _shuttle = Instantiate(shuttlePrefab, this.transform);
        _shuttle.transform.position = this.transform.position;
        _shuttle.transform.rotation = this.transform.rotation;

        var shuttleBehavior = _shuttle.GetComponent<ShuttleBehavior>();
        shuttleBehavior.spacePortOfOrigin = this.gameObject;
        shuttleBehavior.OnDestroyEvent += () =>
        {
            _shuttle = null;
            SpawnShuttle();
        };
        ResourceManagerBehavior.Instance.AddResource(Resource.SHUTTLES, 1);
        _hasShuttle = true;
    }

    public void StartMission(IShuttleMission mission)
    {
        if (mission.MissionAccepted)
            return;

        if (!_hasShuttle)
            return;

        // Accept this mission
        mission.MissionAccepted = true;

        // Disconnect the shuttle from this transform
        _shuttle.transform.parent = null;
        _hasShuttle = false;
        StartCoroutine(co_launchShuttle(mission));

        ResourceManagerBehavior.Instance.AddResource(Resource.SHUTTLES, -1);

        if (takeoffParticle != null)
            takeoffParticle.Play();

        // Deselect the resource
        SelectionManager.SelectedGameObject = null;
    }

    IEnumerator co_launchShuttle(IShuttleMission mission)
    {
        yield return new WaitForSeconds(1);
        var shuttleBehavior = _shuttle.GetComponent<ShuttleBehavior>();
        ShuttleMissionManager.Instance.StartMission(shuttleBehavior, mission);
    }

    private void OnDestroy()
    {
        ShuttleMissionManager.OnMissionScheduled -= OnMissionScheduled;
    }

    IEnumerator co_PollMissionManager()
    {
        while (this.gameObject != null)
        {
            yield return new WaitForSeconds(1.0f);
            if (ShuttleMissionManager.Instance.missions.Count <= 0)
            {
                continue;
            }

            var mission = ShuttleMissionManager.Instance.missions[0];
            StartMission(mission);
        }

    }
}
