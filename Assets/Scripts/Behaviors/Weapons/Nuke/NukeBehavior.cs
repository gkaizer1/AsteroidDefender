using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class NukeBehavior : MonoBehaviour, IStateMachine
{
    public event Action OnObjectDestroyed;

    public StateManager<NukeBehavior> _state;
    public static List<NukeBehavior> Nukes = new List<NukeBehavior>();

    [Header("Game Settings")]
    public float Acceleration = 0.1f;
    public float maxRotationPerSecond = 180.0f;
    public float maxSpeed = 5.0f;

    [Header("Prefab Settings")]
    public GameObject Explosion1;

    public Rigidbody2D rigidBody;

    public float TimeToLaunch = 2.0f;

    public SpriteRenderer fillableSprit;

    public bool ExplodeAtDestination = false;

    public float LifeTimeSeconds = 3.0f;

    public ParticleSystem smokeParticles = null;
    public bool sizeOnLaunch = true;
    public float launchVelocity = 1.0f;

    System.Action<Vector3> _onExplosion;

    public bool IgnoreCollisions = false;

    public enum DetonationMode
    {
        RANGE,
        POINT
    }

    public DetonationMode detonationMode = DetonationMode.RANGE;

    bool _fired = false;

    // Start is called before the first frame update
    void Awake()
    {
        this.name = $"Nuclear Missle";
        if(_state == null)
            _state = new StateManager<NukeBehavior>(new NukeIdleState(this));
        Nukes.Add(this);
    }
    public string CurrentState => _state.Name;

    public NukeBehavior Fire()
    {
        var takeOff = new NukeTakingOffState(this).OnTakeoffCompleted(() => StartCoroutine(co_TimeToDie()));
        _state = new StateManager<NukeBehavior>(takeOff);
        _fired = true;
        
        return this;
    }

    private void OnDestroy()
    {
        Nukes.Remove(this);
        OnObjectDestroyed?.Invoke();
    }

    IEnumerator co_TimeToDie()
    {
        yield return new WaitForSeconds(LifeTimeSeconds);
        SelfDestruct();
    }

    public void SelfDestruct()
    {
        if (this.smokeParticles != null)
            this.smokeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        _state.SetState(new NukeLostState(this));
    }

    // Update is called once per frame
    void Update()
    {
        _state.Update();        
    }

    public void FixedUpdate()
    {
        // Always keep the velocity pointing in the up direction
        this.rigidBody.velocity = rigidBody.transform.up * this.rigidBody.velocity.magnitude;
        _state.FixedUpdate();
    }

    public NukeBehavior OnExplosion(System.Action<Vector3> onExplosion)
    {
        _onExplosion = onExplosion;
        return this;
    }

    [ContextMenu("Detonate")]
    public void DetonateRange(Vector3 detonationPoint)
    {
        CreateExplosion(detonationPoint);
        _onExplosion?.Invoke(detonationPoint);
        Destroy(this.gameObject);
    }

    public void DetonatePoint(Vector3 detonationPoint, Collider2D collider)
    {
        CreateExplosion(detonationPoint);
        _onExplosion?.Invoke(detonationPoint);
        Destroy(this.gameObject);
    }

    public void CreateExplosion(Vector3 explosionPoint)
    {
        GameObject explosion0 = (GameObject)Instantiate(Explosion1);
        explosion0.transform.position = explosionPoint;
        explosion0.transform.localScale = Vector3.one * explosion0.transform.localScale.x;
        explosion0.name = $"{name}_explosion";
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_fired)
            return;

        if (IgnoreCollisions)
            return;

        if (collider.gameObject.GetComponent<EnemyBehavior>() != null)
        {
            ContactPoint2D[] contacts = new ContactPoint2D[1];
            int numContacts = collider.GetContacts(contacts);

            Vector3 detonationPoint = this.transform.position;
            if (numContacts > 0)
            {
                detonationPoint = new Vector3(contacts[0].point.x, contacts[0].point.y, 0f);
            }

            if (detonationMode == DetonationMode.RANGE)
                DetonateRange(detonationPoint);
            else
                DetonatePoint(detonationPoint, collider);
        }
    }
}
