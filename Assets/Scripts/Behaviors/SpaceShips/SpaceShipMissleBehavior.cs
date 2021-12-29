using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpaceShipMissleBehavior : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject attackPrefab = null;

    [Header("Children")]
    public Rigidbody2D rigidBody;
    public List<LaserBeam> laserBeams = new List<LaserBeam>();
    public GameObject warpTrail;

    [Header("Settings")]
    public int Missiles = 3;

    [HideInInspector]
    public Vector3 _returnPoint;

    StateManager<SpaceShipMissleBehavior> _stateManager = null;
    float secondsPerMissle = 2f;

    void Start()
    {
        _returnPoint = this.transform.position;
        this.transform.parent = GameObject.Find("ENEMIES_CONTAINER").transform;

        this.name = $"Spaceship-{Guid.NewGuid().ToString().ToUpper().Substring(0, 6)}";
        this.gameObject.layer = LayerMask.NameToLayer("enemies");

        // Aim at a random point on the earth
        Vector3 _destination = Vector3.zero;
        var earth = GameObject.FindGameObjectWithTag("earth");
        var earthCircle = earth.GetComponent<CircleCollider2D>();

        var state = new EnemySpawningBehavior<SpaceShipMissleBehavior>(this)
        {
            stopAfterSpawn = false
        }
        .OnSpawnCompleted(() =>
        {
            var earth = GameObject.FindGameObjectWithTag("earth");
            var earthCircle = earth.GetComponent<CircleCollider2D>();
            float radius = earthCircle.radius * 1.2f;
            float randomAngle = UnityEngine.Random.Range(0, 360);
            var randomPoint = new Vector2(
                radius * Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                radius * Mathf.Sin(randomAngle * Mathf.Deg2Rad));

            var flyToTarget = new FlyToTargetState<SpaceShipMissleBehavior>(this, randomPoint)
            {
                stopAtTarget = true
            }.OnDestinationReached(() => {
                var delta = this.transform.position;
                var finalRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, -delta));
                this.transform.DORotateQuaternion(finalRotation, 1.0f).OnComplete(() =>
                {
                    StartCoroutine(co_fireMissles());
                });
                return new IdleState<SpaceShipMissleBehavior>(this);
            });
            return flyToTarget;
        });

        _stateManager = new StateManager<SpaceShipMissleBehavior>(state);
    }

    IEnumerator co_fireMissles()
    {
        while (Missiles > 0)
        {
            yield return new WaitForSeconds(secondsPerMissle);
            Missiles--;
            this.GetComponent<EnemyMissleLauncher>().Fire(Utils.GenerateRandomPointInEarth());
        }

        _stateManager.SetState(new EnemyWrapAwayState<SpaceShipMissleBehavior>(this, this._returnPoint)
            .OnWarpStarted(() => warpTrail.SetActive(true))
            .OnWarpedAway(() =>
            {
                Destroy(this.gameObject);
                return null;
            }));
    }

    private void OnDestroy()
    {
        _stateManager?.OnDestroy();
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
