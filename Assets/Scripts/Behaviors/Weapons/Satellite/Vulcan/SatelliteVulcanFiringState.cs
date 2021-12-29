using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SatelliteVulcanFiringState : IState<SatelliteVulcanBehavior>
{
    public override string Name => "Firing";

    GameObject _target = null;
    AutoAimBehavior _autoAimBehavior = null;

    float fireCountDown = 0.0f;

    public SatelliteVulcanFiringState(SatelliteVulcanBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
        _target = Parent.GetComponent<AutoAimBehavior>().currentTarget;
    }

    public override IState<SatelliteVulcanBehavior> Update()
    {
        if (_target == null || _autoAimBehavior.currentTarget != _target)
        {
            return new SatelliteVulcanRotateToTargetState(Parent);
        }

        Vector2 delta = _target.transform.position - Parent.satelliteHead.transform.position;

        // Keep top rotated towards target
        {
            float angle = Vector2.SignedAngle(Vector2.down, delta);
            float angleDelta = Mathf.DeltaAngle(Parent.satelliteHead.transform.rotation.eulerAngles.z, angle);
            float anglePerStep = Parent.RotationAnglePerSecond * Time.deltaTime;
            Parent.satelliteHead.transform.rotation = Quaternion.Euler(
                0f,
                0f,
                Parent.satelliteHead.transform.rotation.eulerAngles.z + angleDelta);
        }

        fireCountDown -= Time.deltaTime;
        if (fireCountDown <= 0)
        {
            fireCountDown = Parent.secondsPerBullet;
            Parent.Fire();
        }

        return this;
    }
}


