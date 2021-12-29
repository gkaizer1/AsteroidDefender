using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RailGunFiringState : IState<RailGunBehavior>
{
    AutoAimBehavior _autoAimBehavior = null;
    GameObject _startTarget = null;
    public RailGunFiringState(RailGunBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
        _startTarget = _autoAimBehavior.currentTarget;
        Parent.CanFire = true;
    }

    public override void Destroy()
    {
        base.Destroy();
        Parent.CancelFire();
        Parent.CanFire = false;
    }

    public override string Name
    {
        get
        {
            return "Firing";
        }
    }

    public override IState<RailGunBehavior> Update()
    {
        base.Update();

        var target = _autoAimBehavior.currentTarget;
        
        // No Target - disable firing state and return to idle state
        if(target == null || target != _startTarget)
        {
            Parent.CanFire = false;
            return new RailGunRotateToTargetState(Parent);
        }


        Vector2 delta = Parent.railGunTop.transform.position - target.transform.position;
        float reachTime = delta.magnitude / (Parent.bulletSpeed * 2.0f);

        Vector3 targetPosition = target.transform.position;
        var targetRigidBody = target.GetComponent<Rigidbody2D>();
        if (targetRigidBody != null)
        {
            // Account for target position
            var targetTravelDistance = (targetRigidBody.velocity) * reachTime;
            targetPosition = targetPosition + new Vector3(targetTravelDistance.x, targetTravelDistance.y, target.transform.position.z);
        }

        // Target ahead to where the object WILL be
        delta = Parent.railGunTop.transform.position - targetPosition;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        // Keep turrent pointing at target
        Parent.railGunTop.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        return this;
    }
}