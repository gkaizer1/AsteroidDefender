using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AstroidOrbitingState : IState<Astroid>
{
    float _timeSinceLastUpdate = 0.0f;
    DateTime _dtIdleStart;
    public AstroidOrbitingState(Astroid parent) : base(parent)
    {
        Init();
    }

    public override void Init()
    {
        _dtIdleStart = DateTime.Now;
        if (Parent.fireEffect != null)
            Parent.fireEffect.SetActive(false);

        Parent.OnCollision += OnAstroidCollision;
        if(Parent._indicator != null)
            Parent._indicator.enabled = false;

        if (Parent.GetComponent<HealthBehavior>() != null)
            Parent.GetComponent<HealthBehavior>().OnHealthChanged.AddListener(OnHealthChanged);

        if (Parent.mainSprite.GetComponent<SpriteRenderer>() != null)
            Parent.mainSprite.GetComponent<SpriteRenderer>().material.EnableKeyword("GREYSCALE_ON");
    }

    private void OnAstroidCollision(GameObject obj)
    {
        // Any distrubance and off we go !
        TransitionToFlying();
    }

    private void OnHealthChanged(float current, float max)
    {
        // Any distrubance and off we go !
        if (current != max)
            TransitionToFlying();
    }

    public void TransitionToFlying()
    {
        // There's a grace period so we don't just start crashing too soon
        if ((DateTime.Now - _dtIdleStart).TotalSeconds < 3.0)
            return;

        _nextState = new AstroidFlyingState(Parent);
        Parent.OnCollision -= OnAstroidCollision;
        if(Parent.GetComponent<HealthBehavior>() != null)
            Parent.GetComponent<HealthBehavior>().OnHealthChanged.RemoveListener(OnHealthChanged);
    }

    public override string Name
    {
        get
        {
            return "Orbiting";
        }
    }

    public override void OnFirstUpdate()
    {
        base.OnFirstUpdate();
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        Parent.rigidBody.velocity = Vector2.one * UnityEngine.Random.Range(0.1f, 0.3f);
        UpdateVelocityToPerp();
    }

    public override void FixedUpdate()
    {
        base.Update();

        // Update the velocity once a second (60 frames * 1,000 astroids adds up !)
        _timeSinceLastUpdate += Time.fixedDeltaTime;
        if (_timeSinceLastUpdate > 1)
        {
            UpdateVelocityToPerp();
            _timeSinceLastUpdate = 0.0f;
        }
    }

    void UpdateVelocityToPerp()
    {
        var velocityMagnitude = Parent.rigidBody.velocity.magnitude;
        var newDirection = Vector3.Cross(Parent.transform.position, Vector3.forward).normalized;
        Parent.rigidBody.velocity = newDirection * velocityMagnitude;
    }
}