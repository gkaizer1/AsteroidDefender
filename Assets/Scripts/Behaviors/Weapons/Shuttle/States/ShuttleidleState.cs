using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShuttleidleState : IState<ShuttleBehavior>
{
    public IState<ShuttleBehavior> nextState = null;

    public ShuttleidleState(ShuttleBehavior parent) : base(parent)
    {
    }

    public override string Name
    {
        get
        {
            return "Idle";
        }
    }

    public override void Init()
    {
        base.Init();
    }
}
