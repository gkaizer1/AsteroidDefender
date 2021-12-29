using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShuttleLaunchingState : IState<ShuttleBehavior>
{
    public IState<ShuttleBehavior> nextState = null;
    float takeOffTime = 5.0f;

    public ShuttleLaunchingState(ShuttleBehavior parent, IState<ShuttleBehavior> next) : base(parent)
    {
        nextState = next;
    }

    public override string Name
    {
        get
        {
            return "Launching";
        }
    }

    public override void Init()
    {
        var rigidBody = Parent.GetComponent<Rigidbody2D>();
        var finalTakeOffVelocity = Parent.transform.up.normalized * 1.0f;
        if(Parent.booster != null && Parent.booster.GetComponent<Animator>() != null)
            Parent.booster.GetComponent<Animator>().SetBool("turn_on_engine", true);

        DOTween.To(
            () => rigidBody.velocity,
            x => rigidBody.velocity = x,
            new Vector2(finalTakeOffVelocity.x, finalTakeOffVelocity.y),
            takeOffTime
        )
        .OnComplete(() =>
        {
            Parent.transform.parent = null;

            // Turn off engines for the booster
            if (Parent.booster != null && Parent.booster.GetComponent<Animator>() != null)
                Parent.booster.GetComponent<Animator>().SetBool("turn_off_engine", true);

            if (Parent.target != null)
            {
                _nextState = nextState;
            }
            else
            {
                Vector3 _initialSize = Parent.transform.localScale;
                _nextState = Parent.CreateReturnBackToBaseState();
            }
        }).SetEase(Ease.InExpo);

    }
}
