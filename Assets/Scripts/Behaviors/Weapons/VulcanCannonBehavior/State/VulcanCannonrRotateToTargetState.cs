using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VulcanCannonrRotateToTargetState : IState<VulcanCannonBehavior>
{
    AutoAimBehavior _autoAimBehavior = null;
    float coolDownTime = 2.0f;
    float coolDownTimer = 2.0f;

    public VulcanCannonrRotateToTargetState(VulcanCannonBehavior obj) : base(obj)
    {
        Init();
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
    }

    public override string Name
    {
        get
        {
            return "Crashing";
        }
    }

    public override IState<VulcanCannonBehavior> Update()
    {
        var target = _autoAimBehavior.currentTarget;
        if (target == null)
        {
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer < 0)
                return new VulcanCannonIdleState(Parent);
            else
                return this;
        }

        // Reset the cooldown
        coolDownTimer = coolDownTime;

        var rotationObject = Parent.cannonTopAnchor;
        Vector2 delta = target.transform.position - rotationObject.transform.position;
        float angle = Vector2.SignedAngle(Vector2.left, delta);
        float angleDelta = Mathf.DeltaAngle(rotationObject.transform.rotation.eulerAngles.z, angle);

        float anglePerStep = Parent.rotationSpeed * Time.deltaTime;

        float rotation = Mathf.Clamp(angleDelta, -anglePerStep, anglePerStep);
        rotationObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationObject.transform.rotation.eulerAngles.z + rotation);

        if (Mathf.Abs(angleDelta) < 1.0f)
        {
            return new VulcanCannonFiringState(Parent);
        }

        return this;
    }
}
