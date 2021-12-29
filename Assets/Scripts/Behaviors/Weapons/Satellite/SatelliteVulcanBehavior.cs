using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SatelliteVulcanBehavior : MonoBehaviour
{
    ObjectPool<GameObject> _bulletPool = null;

    [Header("Prefab")]
    public GameObject bulletPrefab;

    [Header("Children")]
    public GameObject satelliteHead;
    public List<GameObject> bulletFirePoint;
    public float RotationAnglePerSecond = 90.0f;
    int _currentBarrel = 1;

    [Header("Fire Settings")]
    public float secondsPerBullet = 0.2f;
    public float bulletSpeed = 30f;
    public float bulletDamage = 4f;

    IState<SatelliteVulcanBehavior> _state;

    ReloaderBehavior _reloader = null;

    void Awake()
    {
        _bulletPool = new ObjectPool<GameObject>(
           () => Instantiate(bulletPrefab, WeaponsContainerBehavior.Instance.transform), // Create
           x => x.SetActive(true), // Take
           x => x.SetActive(false), // Return to bool
           x => Destroy(x), // Destroy
           true, // Collection checks
           10, // Default size
           250); // Max Size

        _reloader = GetComponent<ReloaderBehavior>();
        //_state = new SatelliteVulcanIdleState(this);
    }

    // Update is called once per frame
    void Update()
    {
        //_state = _state.Update();
    }

    public void Fire()
    {
        var target = GetComponent<AutoAimBehavior>().currentTarget;
        if (_reloader.Ammo < 1.0 || target == null)
            return;

        float lifeTime = GetComponent<AutoAimBehavior>().range / bulletSpeed;

        var bullet = this._bulletPool.Get();
        bullet.GetComponent<RailGunBulletBehavior>().Speed = bulletSpeed;
        bullet.GetComponent<RailGunBulletBehavior>().Damage = bulletDamage;
        bullet.GetComponent<RailGunBulletBehavior>().LifeTime = lifeTime;
        int barrel = _currentBarrel++ % bulletFirePoint.Count;
        var barrelTip = bulletFirePoint[barrel];
        if (barrelTip.GetComponentInChildren<ParticleSystem>() != null)
            barrelTip.GetComponentInChildren<ParticleSystem>().Play();

        var delta = target.transform.position - barrelTip.transform.position;
        Vector3 targetPosition = target.transform.position;
        var targetRigidBody = target.GetComponent<Rigidbody2D>();
        if (targetRigidBody != null)
        {
            float reachTime = delta.magnitude / (bulletSpeed * 2.0f);
            var targetTravelDistance = (targetRigidBody.velocity) * reachTime;
            targetPosition = targetPosition + new Vector3(targetTravelDistance.x, targetTravelDistance.y, target.transform.position.z);
        }
        delta = targetPosition - barrelTip.transform.position;

        float angle = Vector2.SignedAngle(Vector2.left, delta);

        bullet.transform.position = bulletFirePoint[barrel].transform.position;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90.0f);

        _reloader.Ammo--;
    }
}
