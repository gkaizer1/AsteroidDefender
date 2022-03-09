using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipBehavior : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject burnTrail = null;

    [Header("Children")]
    public Rigidbody2D rigidBody;
    public List<LaserBeam> laserBeams = new List<LaserBeam>();
    public GameObject warpTrail;

    [Header("Settings")]
    public float DamagePerSecond = 25.0f;
    public float fireSeconds = 3.0f;

    [HideInInspector]
    public GameObject _attackTarget;
    [HideInInspector]
    public Vector3 _returnPoint;

    StateManager<SpaceShipBehavior> _stateManager = null;

    void Start()
    {
        _returnPoint = this.transform.position;
        this.transform.parent = GameObject.Find("ENEMIES_CONTAINER").transform;

        this.name = $"Spaceship-{Guid.NewGuid().ToString().ToUpper().Substring(0, 6)}";
        this.gameObject.layer = LayerMask.NameToLayer("enemies");

        // Aim at a random point on the earth
        {
            var targetPoint = Utils.GenerateRandomPointInEarth();
            _attackTarget = burnTrail != null ? Instantiate(burnTrail, this.transform.parent) : new GameObject();
            _attackTarget.name = "attack_target_" + this.name;
            _attackTarget.transform.parent = this.transform.parent;
            _attackTarget.transform.position = targetPoint;
        }

        var state = new EnemySpawningBehavior<SpaceShipBehavior>(this)
        {
            stopAfterSpawn = false
        }
        .OnSpawnCompleted(() =>
        {
            var flyToTarget = new FlyToTargetState<SpaceShipBehavior>(this, _attackTarget.transform)
            {
                StopDistance = 1.5f
            }.OnDestinationReached(() => new SpaceShipAttackEarthState(this));
            return flyToTarget;
        });

        _stateManager = new StateManager<SpaceShipBehavior>(state);
    }

    private void OnDestroy()
    {
        _stateManager?.OnDestroy();

        var autoDestroy = _attackTarget.GetComponent<AutoDestroyBehavior>();
        if (autoDestroy == null)
            Destroy(_attackTarget);
        else
            autoDestroy.DestroySelf();
    }

    void Update()
    {
        _stateManager.Update();
    }

    private void FixedUpdate()
    {
        _stateManager.FixedUpdate();
    }
}
