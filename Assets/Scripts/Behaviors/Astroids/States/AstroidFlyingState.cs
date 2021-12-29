using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AstroidFlyingState : IState<Astroid>
{
    float _timeSinceLastUpdate = 0.0f;
    float _initialVelocity = 0.0f;
    public AstroidFlyingState(Astroid parent, float initialVelocity = 0) : base(parent)
    {
        _initialVelocity = initialVelocity;
        Init();
    }

    public override void Init()
    {
        if (Parent.fireEffect != null)
            Parent.fireEffect.SetActive(false);

        if (Parent._indicator != null)
            Parent._indicator.enabled = false;

        if (Parent.mainSprite != null && Parent.mainSprite.GetComponent<SpriteRenderer>() != null)
        {
            Parent.mainSprite.GetComponent<SpriteRenderer>().material.DisableKeyword("GHOST_ON");
            Parent.mainSprite.GetComponent<SpriteRenderer>().material.DisableKeyword("GREYSCALE_ON");
            foreach(string effect in Parent.effectsToEnableOnActivated)
            {
                Parent.mainSprite.GetComponent<SpriteRenderer>().material.EnableKeyword(effect);
            }
        }

        if (Parent.GetComponent<Animator>() != null)
            Parent.GetComponent<Animator>().enabled = true;


        Vector2 delta = Parent._targetPoint - Parent.transform.position;
        Parent.rigidBody.velocity += _initialVelocity * delta.normalized;
    }

    public override string Name
    {
        get
        {
            return "In Transit";
        }
    }

    public override void FixedUpdate()
    {
        if (Parent.IsInsideEarth)
        {
            _nextState = new AstroidCrashingState(Parent);
            return;
        }

        // If astroid is off course - then try to stop the astroid
        if (!Parent.IsOnCourse)
        {
            Vector2 deltaToEarth = Parent.transform.position - Parent._targetPoint;
            float deltaAngle = Vector2.SignedAngle(deltaToEarth, Parent.rigidBody.velocity);
            float maxRotationPerSecond = 45.0f;
            deltaAngle = Mathf.Clamp(deltaAngle, -maxRotationPerSecond, maxRotationPerSecond);

            float rotationAngle = deltaAngle * Time.deltaTime;
            if (Mathf.Abs(rotationAngle) > Mathf.Abs(deltaAngle))
                rotationAngle = deltaAngle;

            Parent.rigidBody.velocity = Utils.rotateVector2(Parent.rigidBody.velocity, rotationAngle);
        }

        // Update the velocity once a second (60 frames * 1,000 astroids adds up !)
        _timeSinceLastUpdate += Time.fixedDeltaTime;
        if (_timeSinceLastUpdate > 1)
        {
            var velocity = Parent.rigidBody.velocity;
            if (velocity.magnitude < 10.0f)
            {
                Vector2 delta = Parent._targetPoint - Parent.transform.position;
                Parent.rigidBody.velocity = velocity + delta.normalized * Parent.settings.Acceleration * _timeSinceLastUpdate;
            }
            _timeSinceLastUpdate = 0.0f;
        }
    }
}