using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EnemySpawningBehavior<T> : IState<T>
     where T : MonoBehaviour
{
    public delegate IState<T> OnSpawnCompletedDelegate();
    public bool stopAfterSpawn = true;
    public float spawnVelocity = 0.4f;

    OnSpawnCompletedDelegate _onSpawned;
    System.Action _onSpawnStarted;
    EnemyBehavior _enemyBehavior = null;
    public override string Name => "Spawning";

    Vector3 _initialPostion;
    Rigidbody2D rigidBody;

    public EnemySpawningBehavior(T parent) : base(parent, false)
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        _enemyBehavior = Parent.GetComponent<EnemyBehavior>();
        if (_enemyBehavior == null)
            Debug.LogError($"No enemy behavior found for [{Parent.name}]");

        spawnVelocity = _enemyBehavior.spawnSpeed;
        stopAfterSpawn = _enemyBehavior.stopAfterSpawn;
    }

    public override void OnFirstUpdate()
    {
        _initialPostion = Parent.transform.position;

        var delta = Vector3.zero - Parent.transform.position;
        Parent.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector3.up, delta));
        
        rigidBody = Parent.GetComponent<Rigidbody2D>();
        rigidBody.velocity = Parent.transform.up * spawnVelocity;
        _enemyBehavior.IsActive = false;

        /*
         * fade in enemies since it's a bit cooler
         */
        SpriteRenderer spriteRender = Parent.GetComponent<SpriteRenderer>();
        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 0.0f);
        spriteRender.DOFade(1.0f, 2.0f);

        _onSpawnStarted?.Invoke();
    }

    public EnemySpawningBehavior<T> OnSpawnCompleted(OnSpawnCompletedDelegate callback)
    {
        _onSpawned = callback;
        return this;
    }

    public EnemySpawningBehavior<T> OnSpawnStarted(System.Action callback)
    {
        _onSpawnStarted = callback;
        return this;
    }

    public override IState<T> Update()
    {
        base.Update();

        var distanceFromSpawn = Vector3.Distance(Parent.transform.position, _initialPostion);

        // Enable targetting after 1 game unit
        if (distanceFromSpawn > 1 && _enemyBehavior.IsActive == false)
            _enemyBehavior.IsActive = true;

        if (distanceFromSpawn < 2.0f)
            return this;

        Parent.GetComponent<EnemyBehavior>().IsActive = true;

        if(stopAfterSpawn)
            rigidBody.velocity = Vector2.zero;

        return _onSpawned.Invoke();
    }
}