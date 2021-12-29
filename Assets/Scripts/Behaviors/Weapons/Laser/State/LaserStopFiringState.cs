using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LaserStopFiringState : IState<LaserBehavior>
{
    bool _started = false;
    public LaserStopFiringState(LaserBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _started = false;
    }

    public override string Name
    {
        get
        {
            return "Cooling Off";
        }
    }

    void FinishFiring()
    {
        Parent.FiringLight.SetActive(false);
        Parent.LaserStart.SetActive(false);
        Parent.LaserEnd.SetActive(false);
        _nextState = new LaserRotateToTargetState(Parent);
    }

    public override IState<LaserBehavior> Update()
    {
        if (!_started)
        {
            _started = true;

            Vector3 laserDelta = Parent.lineRenderer.GetPosition(0) - Parent.lineRenderer.GetPosition(1);
            float laserSpeed = 150.0f;
            float laserReachTime = laserDelta.magnitude / laserSpeed;

            if (laserReachTime > 0.1f)
            {
                // Show start/end effects
                Parent.LaserStart.SetActive(true);
                Parent.LaserEnd.SetActive(true);

                // Tween laser back to end - then return to idle state
                DOTween.To(
                    () => Parent.lineRenderer.GetPosition(0),
                    x =>
                    {
                        Parent.LaserStart.transform.position = x;
                        Parent.lineRenderer.SetPosition(0, x);
                    },
                    Parent.lineRenderer.GetPosition(1),
                    laserReachTime).OnComplete(() =>
                    {
                        FinishFiring();
                    });
            }
            else
            {
                FinishFiring();
            }
        }

        return _nextState;
    }
}