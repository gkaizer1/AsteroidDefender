using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Pool;

public class RailGunBulletBehavior : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private GameObject hitExplosion;

    [SerializeField]
    private GameObject hitParticleSystem;

    public float Speed = 15.0f;

    public float Damage = 75.0f;

    public float LifeTime = 2.0f;

    Tween velocityTween = null;

    public ObjectPool<GameObject> Pool = null;

    bool _isHit = false;

    public void Fire()
    {
        _isHit = false;
        var finalSpeed = this.transform.up.normalized * Speed;
        var finalSpeedVector = new Vector2(finalSpeed.x, finalSpeed.y);

        // Bullets start out TOO fast and slow down - due to 'fun'
        rigidBody.velocity = finalSpeedVector * 2.0f;

        velocityTween = DOTween.To(
            () => rigidBody.velocity,
            x => rigidBody.velocity = x,
            finalSpeedVector,
            0.2f).OnComplete(() =>
            {
                velocityTween = null;
            }).SetRecyclable(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var damagables = collision.gameObject.GetComponents<HealthBehavior>().Where(x => x.Health > 0);
        if (!damagables.Any())
            return;

        var damagable = damagables.First();

        if (damagable.Health <= 0)
            return;

        // Do not allow hitting multiple enemies
        if (_isHit)
            return;

        _isHit = true;

        if (velocityTween != null)
            velocityTween.Kill(false);

        damagable.DoDamage(Damage);

        if (hitExplosion != null)
        {
            ContactPoint2D[] contacts = new ContactPoint2D[1];
            int numContacts = collision.GetContacts(contacts);
            Vector3 detonationPoint = this.transform.position;
            if (numContacts > 0)
            {
                detonationPoint = new Vector3(contacts[0].point.x, contacts[0].point.y, 0f);
            }

            var explosion = Instantiate(hitExplosion);
            explosion.transform.position = detonationPoint;
            explosion.name = $"explosion_{this.name}";
            explosion.transform.localScale = Vector3.one * 0.5f;

            if(hitParticleSystem != null)
            {
                var particleSystem = GameObject.Instantiate(this.hitParticleSystem, this.transform.parent);
                particleSystem.transform.position = this.transform.position;
                float angle = Mathf.Atan2(this.rigidBody.velocity.y, this.rigidBody.velocity.x) * Mathf.Rad2Deg;
                particleSystem.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        rigidBody.velocity = Vector2.zero;
        if (Pool == null)
            Destroy(this.gameObject);
        else
            Pool.Release(this.gameObject);
    }
}
