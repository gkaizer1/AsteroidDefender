using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceFigherBehavior : MonoBehaviour
{
    StateManager<EnemySpaceFigherBehavior> _stateManager;
    Vector3 _startPostion;
    Vector2 _targetPoint;
    Vector3 _debugPnt;
    Seeker _seeker;

    void DisableTrails()
    {
        foreach (var trail in this.GetComponentsInChildren<TrailRenderer>())
        {
            trail.enabled = false;
        }
    }
    void EnableTrails()
    {
        foreach (var trail in this.GetComponentsInChildren<TrailRenderer>())
        {
            trail.enabled = true;
        }
    }

    void Start()
    {
        _startPostion = this.transform.position;
        _targetPoint = Utils.GenerateRandomPointInEarth();
        DisableTrails();
        _debugPnt = Utils.GenerateClosestPointToEarth(this.transform.position);

        var initialState = new EnemySpawningBehavior<EnemySpaceFigherBehavior>(this)
            .OnSpawnCompleted(() =>
            {
                EnableTrails();
                return new FlyToSeekerBehavior<EnemySpaceFigherBehavior>(this, Utils.GenerateClosestPointToEarth(this.transform.position))
                {
                    StopDistance = 1.0f
                }
                .OnDestinationReached(() =>
                {
                    return new FlyToTargetState<EnemySpaceFigherBehavior>(this, _targetPoint)
                    {
                        StopDistance = 2.0f
                    }.OnDestinationReached(() =>
                    {
                        this.GetComponent<EnemyMissleLauncher>().Fire(_targetPoint);
                        return new EnemyWrapAwayState<EnemySpaceFigherBehavior>(this, this._startPostion)
                                .OnWarpedAway(() =>
                                {
                                    Destroy(this.gameObject);
                                    return null;
                                });
                    });
                });
            });

        _stateManager = new StateManager<EnemySpaceFigherBehavior>(initialState);
    }

    private void OnDestroy()
    {
        _stateManager.OnDestroy();
    }

    private void Update()
    {
        _stateManager.Update();
    }

    private void FixedUpdate()
    {
        _stateManager.FixedUpdate();
    }
}