using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NukeFlyingState : IState<NukeBehavior>
{
    public Rigidbody2D rigidBody;
    public AutoAimBehavior autoAimBehavior;

    public NukeFlyingState(NukeBehavior parent) : base(parent)
    {
    }

    public override void Init()
    {
        rigidBody = Parent.GetComponent<Rigidbody2D>();
        autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();

        // Disable the launcher mask
        Parent.GetComponentInChildren<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

        if (Parent.smokeParticles != null)
            Parent.smokeParticles.Play();
    }

    void UpdateAcceleration()
    {
        if(rigidBody.velocity.magnitude < Parent.maxSpeed)
            rigidBody.velocity += new Vector2(Parent.transform.up.x, Parent.transform.up.y) * Parent.Acceleration * Time.deltaTime;
    }

    public override string Name
    {
        get
        {
            return "Flying";
        }
    }

    public override IState<NukeBehavior> Update()
    {
        if (autoAimBehavior.currentTarget != null)
        {
            UpdateAcceleration();

            Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);
            Vector2 destination = new Vector2(autoAimBehavior.currentTarget.transform.position.x, autoAimBehavior.currentTarget.transform.position.y);
            Vector2 delta = destination - currentPostion;

            float deltaAngle = Vector2.SignedAngle(Parent.transform.up, delta.normalized);
            this.rigidBody.rotation += Mathf.Clamp(deltaAngle, -Parent.maxRotationPerSecond * Time.deltaTime, Parent.maxRotationPerSecond * Time.deltaTime);

            if(Parent.ExplodeAtDestination)
            {
                if (delta.magnitude < 0.2f)
                {
                    Parent.transform.position = destination;
                    Parent.DetonateRange(autoAimBehavior.currentTarget.transform.position);
                }
            }
        }

        return this;
    }
}
