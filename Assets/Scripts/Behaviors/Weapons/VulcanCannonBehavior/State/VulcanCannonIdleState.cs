using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class VulcanCannonIdleState : IState<VulcanCannonBehavior>
{
    AutoAimBehavior _autoAimBehavior = null;
    RotatorBehavior _rotator = null;

    public VulcanCannonIdleState(VulcanCannonBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = null;
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();

        // Enable idle rotation
        _rotator = Parent.cannonTopAnchor.GetComponent<RotatorBehavior>();
        _rotator.enabled = true;
    }

    public override string Name
    {
        get
        {
            return "Idle";
        }
    }

    public override IState<VulcanCannonBehavior> Update()
    {
        if (_autoAimBehavior.currentTarget != null)
        {
            _rotator.enabled = false;
            return new VulcanCannonrRotateToTargetState(Parent);
        }

        return this;
    }
}
