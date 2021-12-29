using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VulcanCannonFiringState : IState<VulcanCannonBehavior>
{
    AutoAimBehavior _autoAimBehavior = null;
    GameObject _target = null;

    public VulcanCannonFiringState(VulcanCannonBehavior obj) : base(obj)
    {
    }

    public  override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
        if(_autoAimBehavior != null)
            _target = _autoAimBehavior.currentTarget;

        Fire();

        float fireRate = 1.0f / Parent.BulletsPerSecond;
        Parent.InvokeRepeating("Fire", fireRate, fireRate);
    }

    public override string Name
    {
        get
        {
            return "Firing";
        }
    }

    public override IState<VulcanCannonBehavior> Update()
    {        
        // No Target - disable firing state and return to idle state
        if(_target == null || _target != _autoAimBehavior.currentTarget)
        {
            Parent.CancelInvoke("Fire");
            return new VulcanCannonrRotateToTargetState(Parent);
        }

        var rotationObject = Parent.cannonTopAnchor;
        Vector2 delta = _target.transform.position - rotationObject.transform.position;
        float angle = Vector2.SignedAngle(Vector2.left, delta);

        // Keep turrent pointing at target
        rotationObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        return this;
    }

    public void Fire()
    {
        Parent.Fire();
    }
}