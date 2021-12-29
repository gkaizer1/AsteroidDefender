using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SatelliteVulcanRotateToTargetState : IState<SatelliteVulcanBehavior>
{
    public override string Name => "Acquiring Target";

    AutoAimBehavior _autoAimBehavior = null;
    float coolDownTimer = 2.0f;
    float coolDownTime = 2.0f;

    public SatelliteVulcanRotateToTargetState(SatelliteVulcanBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
    }

    public override IState<SatelliteVulcanBehavior> Update()
    {
        var target = _autoAimBehavior.currentTarget;
        if (target == null)
        {
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer < 0)
                return new SatelliteVulcanIdleState(Parent);
            else
                return this;
        }

        // Reset the cooldown
        coolDownTimer = coolDownTime;

        Vector2 delta = target.transform.position - Parent.satelliteHead.transform.position;
        float angle = Vector2.SignedAngle(Vector2.down, delta);
        float angleDelta = Mathf.DeltaAngle(Parent.satelliteHead.transform.rotation.eulerAngles.z, angle);

        float anglePerStep = Parent.RotationAnglePerSecond * Time.deltaTime;

        float rotation = Mathf.Clamp(angleDelta, -anglePerStep, anglePerStep);
        Parent.satelliteHead.transform.rotation = Quaternion.Euler(
            0f,
            0f,
            Parent.satelliteHead.transform.rotation.eulerAngles.z + rotation);

        if (Mathf.Abs(angleDelta) < 0.2f)
        {
            return new SatelliteVulcanFiringState(Parent);

        }

        return this;
    }
}