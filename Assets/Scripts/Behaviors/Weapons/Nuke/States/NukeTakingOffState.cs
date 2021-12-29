using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NukeIdleState : IState<NukeBehavior>
{
    public NukeIdleState(NukeBehavior parent) : base(parent)
    {
    }

    public override string Name => "Idle";
}

public class NukeTakingOffState : IState<NukeBehavior>
{
    System.Action _onCompleted;
    public NukeTakingOffState(NukeBehavior parent) : base(parent)
    {
    }

    public override string Name
    {
        get
        {
            return "Taking Off";
        }
    }

    public NukeTakingOffState OnTakeoffCompleted(System.Action onCompleted)
    {
        _onCompleted = onCompleted;
        return this;
    }

    public override void OnFirstUpdate()
    {
        var nukeBehavior = Parent.GetComponent<NukeBehavior>();

        // Increase parent size
        if(Parent.sizeOnLaunch)
        {
            var parentIntialScale = Parent.transform.localScale;
            Parent.transform.localScale = parentIntialScale * 0.5f;
            Parent.transform.DOScale(parentIntialScale, nukeBehavior.TimeToLaunch);
        }

        var finalVelocity = Parent.transform.up * Parent.launchVelocity;
        DOTween.To(
            () => Parent.rigidBody.velocity,
            x => Parent.rigidBody.velocity = x,
            new Vector2(finalVelocity.x, finalVelocity.y),
            nukeBehavior.TimeToLaunch)
        .OnComplete(() =>
            {
                _nextState = new NukeFlyingState(Parent);
                _onCompleted?.Invoke();
            });
    }
}
