using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AstroidDyingState : IState<Astroid>
{
    public AstroidDyingState(Astroid parent) : base(parent)
    {

    }

    public override void Init()
    {
    }

    public override string Name
    {
        get
        {
            return "Crashing";
        }
    }

    public override IState<Astroid> Update()
    {
        return _nextState;
    }
}