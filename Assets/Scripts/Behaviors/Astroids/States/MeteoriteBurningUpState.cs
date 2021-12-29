using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MeteoriteBurningUpState : IState<Astroid>
{
    float crashTimeSeconds = 3.0f;
    float timeSinceEnteredEarth = 0.0f;

    float startSize = 1.0f;

    public MeteoriteBurningUpState(Astroid parent) : base(parent)
    {
    }

    public override string Name
    {
        get
        {
            return "Burning Up";
        }
    }

    public override void Init()
    {
        startSize = Parent.transform.localScale.x;

        Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
        Vector2 destination = new Vector2(Parent._targetPoint.x, Parent._targetPoint.y);
        Vector2 delta = destination - currentPostion;

        crashTimeSeconds = (delta.magnitude / Parent.rigidBody.velocity.magnitude);
        timeSinceEnteredEarth = 0.0f;

        if (Parent.fireEffect != null)
        {
            Parent.fireEffect.SetActive(true);
            Parent.fireEffect.transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, -Parent.rigidBody.velocity));
        }
    }

    public override IState<Astroid> Update()
    {
        base.Update();

        timeSinceEnteredEarth += Time.deltaTime;
        if (timeSinceEnteredEarth > crashTimeSeconds)
        {
            GameObject.Destroy(Parent.gameObject);
            return _nextState;
        }

        float precentOfRadius = Mathf.Clamp01(timeSinceEnteredEarth / crashTimeSeconds);
        float newScale = startSize * (1.0f - precentOfRadius);
        float newSize = Mathf.Clamp(newScale, 0.0f, 1.0f);
        Parent.transform.localScale = Vector3.one * newSize;

        return _nextState;
    }
}