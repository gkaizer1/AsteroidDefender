using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AstroidCrashingState : IState<Astroid>
{
    float crashTimeSeconds = 3.0f;
    float timeSinceEnteredEarth = 0.0f;

    float startSize = 1.0f;

    int no_collision_layer = 0;
    int astroid_layer = 0;

    public AstroidCrashingState(Astroid parent) : base(parent)
    {
    }

    public override string Name
    {
        get
        {
            return "Crashing";
        }
    }

    public override void Init()
    {
        startSize = Parent.transform.localScale.x;

        // accelerate to 10 game units
        Parent.rigidBody.velocity = Parent.rigidBody.velocity.normalized * 10.0f;

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

        //if (Parent._indicator != null)
        //    Parent._indicator.enabled = true;

        Parent.GetComponent<EnemyBehavior>().IsActive = false;

        no_collision_layer = LayerMask.NameToLayer("astroids_no_collisions");
        astroid_layer = LayerMask.NameToLayer("Astroids");
        if (Parent.gameObject.layer != no_collision_layer)
            Parent.gameObject.layer = no_collision_layer;
    }

    public override IState<Astroid> Update()
    {
        timeSinceEnteredEarth += Time.deltaTime;

        // Astroid got pushed back from earth's radius - start flying back
        if (!Parent.IsInsideEarth)
        {
            if (Parent.gameObject.layer != astroid_layer)
                Parent.gameObject.layer = astroid_layer;
            return new AstroidFlyingState(Parent);
        }

        if (timeSinceEnteredEarth > crashTimeSeconds)
            return new AstroidExplodingState(Parent);

        float minSize = Mathf.Clamp(0.5f, 0.5f, startSize);

        float precentOfRadius = Mathf.Clamp01(timeSinceEnteredEarth / crashTimeSeconds);
        float newScale = startSize * (1.0f - precentOfRadius);

        float newSize = Mathf.Clamp(newScale, minSize, 1.0f);
        //Parent.transform.localScale = Vector3.one * newSize;

        return _nextState;
    }
}