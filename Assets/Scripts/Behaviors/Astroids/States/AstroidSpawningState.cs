using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AstroidSpawningState : IState<Astroid>
{
    IState<Astroid> _next = null;
    public AstroidSpawningState(Astroid parent, IState<Astroid> nextState) : base(parent)
    {
        Init();
        _next = nextState;
    }

    public override string Name => "Spawning";

    public override void Init()
    {
        base.Init();
        var sprite = Parent.mainSprite.GetComponent<SpriteRenderer>();
        if (sprite == null)
            _nextState = _next;
        else
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
            sprite.DOFade(1.0f, 1.0f).OnComplete(() =>
            {
                _nextState = _next;
            });
        }
    }
}