using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EnemyWrapAwayState<T> : IState<T>
     where T : MonoBehaviour
{
    public delegate IState<T> OnWarpEndedDelegate();

    public override string Name => "Warping";
    Vector3 _returnPoint;
    System.Action _OnWarpStarted;
    OnWarpEndedDelegate _OnWarpEnded;

    public EnemyWrapAwayState(T parent, Vector3 returnPoint) : base(parent, false)
    {
        _returnPoint = returnPoint;
        Init();
    }

    public override void Init()
    {
        base.Init();
        var enemyBehavior = Parent.GetComponent<EnemyBehavior>();
        if (enemyBehavior != null)
            enemyBehavior.IsActive = false;
    }

    public EnemyWrapAwayState<T> OnWarpStarted(System.Action onWarpStarted)
    {
        _OnWarpStarted = onWarpStarted;
        return this;
    }

    public EnemyWrapAwayState<T> OnWarpedAway(OnWarpEndedDelegate onWarpEnded)
    {
        _OnWarpEnded = onWarpEnded;
        return this;
    }

    public override void OnFirstUpdate()
    {
        _OnWarpStarted?.Invoke();

        // This is no longer an valid target
        if(Parent.GetComponent<EnemyBehavior>() != null)
            Parent.GetComponent<EnemyBehavior>().IsActive = false;

        var delta = Parent.transform.position - _returnPoint;
        var rigidBody = Parent.GetComponent<Rigidbody2D>();
        rigidBody.velocity = Vector2.zero;
        Parent.transform.DORotateQuaternion(Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Parent.transform.position, _returnPoint)), 1.0f).OnComplete(() =>
        {
            DOTween.To(
                () => rigidBody.velocity,
                x => rigidBody.velocity = x,
                new Vector2(Parent.transform.up.x, Parent.transform.up.y) * 100.0f,
                1f).OnComplete(() =>
                {
                    this._nextState = _OnWarpEnded?.Invoke();
                });
        });

    }
}