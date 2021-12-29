using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LaserIdleState : IState<LaserBehavior>
{
    AutoAimBehavior _autoAimBehavior = null;
    RotatorBehavior _rotator = null;

    public LaserIdleState(LaserBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = null;
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();

        _rotator = Parent.laserTopAnchor.GetComponent<RotatorBehavior>();
        _rotator.enabled = true;

        Parent.LaserStart.SetActive(false);
        Parent.LaserEnd.SetActive(false);
        Parent.lineRenderer.enabled = false;
    }

    public override string Name
    {
        get
        {
            return "Idle";
        }
    }

    public override IState<LaserBehavior> Update()
    {
        if (_autoAimBehavior.currentTarget != null)
        {
            Parent.LaserStart.SetActive(false);
            Parent.LaserEnd.SetActive(false);
            _rotator.enabled = false;
            return new LaserRotateToTargetState(Parent);
        }

        return this;
    }
}
