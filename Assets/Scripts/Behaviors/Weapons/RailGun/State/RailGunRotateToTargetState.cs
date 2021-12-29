using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RailGunRotateToTargetState : IState<RailGunBehavior>
{
    AutoAimBehavior _autoAimBehavior = null;
    float coolDownTime = 2.0f;
    float coolDownTimer = 2.0f;

    public RailGunRotateToTargetState(RailGunBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
    }

    public override string Name
    {
        get
        {
            return "Targeting";
        }
    }

    public override IState<RailGunBehavior> Update()
    {
        var target = _autoAimBehavior.currentTarget;
        if (target == null)
        {
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer < 0)
                return new RailGunIdleState(Parent);
            else
                return this;
        }

        // Reset the cooldown
        coolDownTimer = coolDownTime;

        var rotationObject = Parent.railGunTop;
        Vector2 delta = target.transform.position - rotationObject.transform.position;
        float angle = Vector2.SignedAngle(Vector2.left, delta);
        float angleDelta = Mathf.DeltaAngle(rotationObject.transform.rotation.eulerAngles.z, angle);

        float anglePerStep = Parent.RotationAnglePerSecond * Time.deltaTime;

        float rotation = Mathf.Clamp(angleDelta, -anglePerStep, anglePerStep);
        rotationObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationObject.transform.rotation.eulerAngles.z + rotation);

        if (Mathf.Abs(angleDelta) < 0.2f)
        {
             return new RailGunFiringState(Parent);
            
        }

        return this;
    }
}
