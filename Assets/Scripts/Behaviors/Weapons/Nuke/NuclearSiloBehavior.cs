using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class NuclearSiloBehavior : MonoBehaviour, IStateMachine
{
    [SerializeField]
    private GameObject SiloSprite;

    [SerializeField]
    private GameObject NukePrefab;

    public float PowerToLauch = 10.0f;

    public ParticleSystem nukeLaunchExhaust = null;

    [Header("Settings")]
    public float MissleDamage = 50f;
    public float DetonationRange = 2.0f;

    public string CurrentState => "Active";

    AutoAimBehavior _autoAimBehavior = null;

    private void Awake()
    {
        _autoAimBehavior = GetComponent<AutoAimBehavior>();
    }

    public void Fire()
    {
        if (GetComponent<ReloaderBehavior>().Ammo < 1.0f)
            return;

        if (GetComponent<PowerConsumer>().CurrentPower < PowerToLauch)
            return;

        if (_autoAimBehavior.currentTarget == null)
            return;

        var nuke = Instantiate(NukePrefab, WeaponsContainerBehavior.Instance.transform);
        nuke.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        nuke.GetComponent<AutoAimBehavior>().currentTarget = _autoAimBehavior.currentTarget;

        nuke.GetComponent<NukeBehavior>()
            .Fire()
            .OnExplosion((explosionPoint) =>
        {
            List<int> physicsMask = new List<int>()
            {
                1 << LayerMask.NameToLayer("Astroids"),
                1 << LayerMask.NameToLayer("astroids_no_collisions"),
                1 <<  LayerMask.NameToLayer("enemies"),
            };

            List<HealthBehavior> colliderObject = new List<HealthBehavior>();

            foreach (var mask in physicsMask)
            {
                /*
                 * Compute all items (implementing IDamagable) that have collided with this explosion & apply a force
                 */
                var itemsInRadius = Physics2D.OverlapCircleAll(explosionPoint, DetonationRange, mask);
                foreach (Collider2D collider in itemsInRadius)
                {
                    var damagables = collider.GetComponents<HealthBehavior>().ToList();
                    if (damagables.Count <= 0)
                        continue;

                    foreach (var damagable in damagables)
                    {
                        var direction = damagable.transform.position - explosionPoint;

                        // Magnitude of force 0 - 1
                        float forceMagnitude = Mathf.Clamp01(1.0f / direction.magnitude);

                        Vector3 forceVelociy = (direction.normalized * forceMagnitude * 1.0f);
                        collider.attachedRigidbody.velocity += new Vector2(forceVelociy.x, forceVelociy.y);
                        damagable.DoDamage(MissleDamage * forceMagnitude);
                    }
                }
            }
        });

        // Decrement ammo
        GetComponent<ReloaderBehavior>().Ammo--;
        GetComponent<PowerConsumer>().CurrentPower -= PowerToLauch;

        if(nukeLaunchExhaust != null)
            nukeLaunchExhaust.Play();
    }

    public void OnTargetChanged(GameObject target)
    {
        if (target == null)
            return;

        Fire();
    }
}
