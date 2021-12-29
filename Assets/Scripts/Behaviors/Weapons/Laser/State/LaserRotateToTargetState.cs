using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LaserRotateToTargetState : IState<LaserBehavior>
{
    AutoAimBehavior _autoAimBehavior = null;
    float coolDownTime = 2.0f;
    float coolDownTimer = 2.0f;

    public LaserRotateToTargetState(LaserBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();

        Parent.LaserStart.SetActive(false);
        Parent.LaserEnd.SetActive(false);
        Parent.lineRenderer.enabled = false;
    }

    public override string Name
    {
        get
        {
            return "Acquiring Target";
        }
    }

    public override IState<LaserBehavior> Update()
    {
        var target = _autoAimBehavior.currentTarget;
        if (target == null)
        {
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer < 0)
                return new LaserIdleState(Parent);
            else
                return this;
        }
        // Reset the cooldown
        coolDownTimer = coolDownTime;

        var rotationObject = Parent.laserTopAnchor;
        Vector2 delta = target.transform.position - rotationObject.transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, delta);
        float angleDelta = Mathf.DeltaAngle(rotationObject.transform.rotation.eulerAngles.z, angle);

        float anglePerStep = Parent.RotationAnglePerSecond * Time.deltaTime;

        float rotation = Mathf.Clamp(angleDelta, -anglePerStep, anglePerStep);
        rotationObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationObject.transform.rotation.eulerAngles.z + rotation);

        // Only fire once we have enough power
        if (Mathf.Abs(angleDelta) < 0.5f)
        {
            return new LaserFiringState(Parent);            
        }

        return this;
    }
}
