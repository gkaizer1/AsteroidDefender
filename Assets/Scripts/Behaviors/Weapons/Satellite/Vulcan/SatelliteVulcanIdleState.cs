using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SatelliteVulcanIdleState : IState<SatelliteVulcanBehavior>
{
    public override string Name => "Idle";

    AutoAimBehavior _autoAimBehavior = null;
    RotatorBehavior _rotator = null;

    public SatelliteVulcanIdleState(SatelliteVulcanBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
        _rotator = Parent.satelliteHead.GetComponent<RotatorBehavior>();
        _rotator.enabled = true;
    }

    public override IState<SatelliteVulcanBehavior> Update()
    {
        if (_autoAimBehavior.currentTarget != null)
        {
            _rotator.enabled = false;
            return new SatelliteVulcanRotateToTargetState(Parent);
        }

        return this;
    }
}