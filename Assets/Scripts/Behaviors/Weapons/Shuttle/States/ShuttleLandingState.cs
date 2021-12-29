using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShuttleLandingState : IState<ShuttleBehavior>
{
    bool _started = false;

    public Action _onLanded;

    public ShuttleLandingState(ShuttleBehavior parent) : base(parent)
    {

    }

    public ShuttleLandingState OnLanded(Action onLanded)
    {
        _onLanded = onLanded;
        return this;
    }

    public override void Init()
    {
        _started = false;
    }

    public override string Name
    {
        get
        {
            return "Landing";
        }
    }

    public override IState<ShuttleBehavior> Update()
    {
        if (!_started)
        {
            _started = true;
            Parent.transform.DOScale(Vector3.zero, 2.0f).OnComplete(() =>
            {
                if (_onLanded != null)
                    _onLanded();

                GameObject.Destroy(Parent.gameObject);
            });
            Parent.GetComponent<SpriteRenderer>().DOFade(0.1f, 1.9f);
        }

        return this;
    }
}
