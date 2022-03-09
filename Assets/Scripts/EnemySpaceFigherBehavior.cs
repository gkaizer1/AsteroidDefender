using DG.Tweening;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAtSatelliteState : IState<EnemySpaceFigherBehavior>
{
    GameObject _target = null;
    public float secondToFire = 3.0f;
    private float _timeSinceLastFired = 0.0f;
    System.Action<GameObject> _onFire;
    GameObject _targetIndicatorInstance;
    Tween _tween = null;
    bool _canFire = false;

    public FireAtSatelliteState(EnemySpaceFigherBehavior parent, GameObject target) : base(parent)
    {
        _target = target;
    }

    public override void Destroy()
    {
        base.Destroy();
        if (_targetIndicatorInstance != null)
            GameObject.Destroy(_targetIndicatorInstance);
        if (_tween != null && _tween.IsPlaying())
            _tween.Kill();
    }

    public override void OnFirstUpdate()
    {
        base.OnFirstUpdate();

        if(Parent.targetIndicatorPrefab)        
            _targetIndicatorInstance = GameObject.Instantiate(Parent.targetIndicatorPrefab, _target.transform);        

        // Slow down and stop !
        var rigidbody = Parent.GetComponent<Rigidbody2D>();
        var currentVelocity = rigidbody.velocity;
        if (currentVelocity.magnitude > float.Epsilon)
        {
            var originalVelocity = rigidbody.velocity;
            _tween = DOTween.To(() => rigidbody.velocity.magnitude,
                        x => rigidbody.velocity = originalVelocity * x,
                        0.0f,
                        1.0f)
                            .OnComplete(() =>
                                {
                                    Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
                                    Vector2 destination = new Vector2(_target.transform.position.x, _target.transform.position.y);
                                    Vector2 delta = destination - currentPostion;

                                    float deltaAngle = Vector2.SignedAngle(Parent.transform.up, delta);
                                    _tween = Parent.GetComponent<Rigidbody2D>()
                                        .DORotate(Parent.GetComponent<Rigidbody2D>().rotation + deltaAngle, Mathf.Abs(deltaAngle / 180.0f) * 5.0f)
                                        .OnComplete(() => _canFire = true);
                                });
        }
    }

    public IState<EnemySpaceFigherBehavior> OnFire(System.Action<GameObject> onFire)
    {
        _onFire = onFire;
        return this;
    }

    public override IState<EnemySpaceFigherBehavior> Update()
    {
        if (_canFire)
        {
            _timeSinceLastFired += Time.deltaTime;
            if (_timeSinceLastFired > secondToFire)
            {
                _onFire?.Invoke(_target);
                _timeSinceLastFired = 0.0f;
            }
        }

        return base.Update();
    }

    public override string Name => "FireAtSatellite";
}

public class EnemySpaceFigherBehavior : MonoBehaviour
{
    StateManager<EnemySpaceFigherBehavior> _stateManager;
    Vector3 _startPostion;
    Vector2 _targetPoint;

    public GameObject targetIndicatorPrefab;
    public GameObject prefabCorpse;

    void ToggleTrails(bool enable)
    {
        foreach (var trail in this.GetComponentsInChildren<TrailRenderer>())
        {
            trail.enabled = enable;
        }
    }

    void Start()
    {
        this.name = $"Enemy-{Guid.NewGuid().ToString().ToUpper().Substring(0, 6)}";
        _startPostion = this.transform.position;
        _targetPoint = Utils.GenerateRandomPointInEarth();
        ToggleTrails(false);

        var initialState = new EnemySpawningBehavior<EnemySpaceFigherBehavior>(this)
            .OnSpawnStarted(() =>
            {
                var metrial = this.GetComponent<SpriteRenderer>().material;
                if (metrial.HasProperty("_ChromAberrAmount"))
                {
                    metrial.SetFloat("_ChromAberrAmount", 1.0f);
                    DG.Tweening.DOTween.To(
                        () => metrial.GetFloat("_ChromAberrAmount"),
                        x => metrial.SetFloat("_ChromAberrAmount", x),
                        0.0f,
                        5.0f);
                }
            })
            .OnSpawnCompleted(() =>
            {
                ToggleTrails(true);
                return new FlyToTargetState<EnemySpaceFigherBehavior>(this, Utils.GenerateClosestPointToEarth(this.transform.position))
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

    IState<EnemySpaceFigherBehavior> _resumeState = null;
    public void AttackObject(GameObject target)
    {
        Debug.Log($"Enemy {name} is attacking {target.name}");
        _resumeState = this._stateManager._state;
        this._stateManager.SetState(new FireAtSatelliteState(this, target).OnFire((target) =>
        {
            this.GetComponent<EnemyMissleLauncher>().Fire(target.transform.position);
        }));
    }

    public void ResumeState()
    {
        if(_resumeState != null)
            this._stateManager.SetState(_resumeState);
    }

    private void OnDestroy()
    {
        _stateManager.OnDestroy();
        if (prefabCorpse != null)
        {
            var corpse = GameObject.Instantiate(prefabCorpse, this.transform.parent);
            corpse.transform.position = this.transform.position;
            corpse.transform.rotation = this.transform.rotation;
            corpse.GetComponent<Rigidbody2D>().velocity = this.transform.up * 0.1f;

        }
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