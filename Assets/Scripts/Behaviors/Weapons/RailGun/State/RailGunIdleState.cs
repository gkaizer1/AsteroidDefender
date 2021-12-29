using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RailGunOutOfPowerState : IState<RailGunBehavior>
{
    RotatorBehavior _rotator = null;

    public RailGunOutOfPowerState(RailGunBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _rotator = Parent.railGunTop.GetComponent<RotatorBehavior>();
        _rotator.enabled = false;
    }

    public override string Name
    {
        get
        {
            return "Out Of Power";
        }
    }

    public override IState<RailGunBehavior> Update()
    {
        return this;
    }
}

public class RailGunIdleState : IState<RailGunBehavior>
{
    AutoAimBehavior _autoAimBehavior = null;
    RotatorBehavior _rotator = null;

    public RailGunIdleState(RailGunBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = null;
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
        _rotator = Parent.railGunTop.GetComponent<RotatorBehavior>();
        _rotator.enabled = true;
    }

    public override string Name
    {
        get
        {
            return "Idle";
        }
    }

    public override IState<RailGunBehavior> Update()
    {
        if (_autoAimBehavior.currentTarget != null)
        {
            _rotator.enabled = false;
            return new RailGunRotateToTargetState(Parent);
        }

        return this;
    }
}
