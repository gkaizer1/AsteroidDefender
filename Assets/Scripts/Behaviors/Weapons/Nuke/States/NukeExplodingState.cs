using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NukeExplodingState : IState<NukeBehavior>
{
    public NukeExplodingState(NukeBehavior parent) : base(parent)
    {
    }

    public override void Init()
    {
    }

    public override string Name
    {
        get
        {
            return "Exploding";
        }
    }

    public override IState<NukeBehavior> Update()
    {
        return _nextState;
    }
}
