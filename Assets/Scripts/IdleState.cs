using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class IdleState<T> : IState<T>
     where T : MonoBehaviour
{
    public IdleState(T parent) : base(parent)
    {
    }

    public override string Name => "Idle";
}