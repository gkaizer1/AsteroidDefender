using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Pool;

public class VulcanCannonBehavior : MonoBehaviour, IStateMachine
{

    StateManager<VulcanCannonBehavior> _state;

    [Header("Children")]
    public GameObject cannonTopAnchor;
    public List<Transform> cannonTips = new List<Transform>();
    public GameObject topCannon;
    public GameObject middleCannon;
    public GameObject bottomCannon;
    public GameObject radar;
    public GameObject ammoBox;

    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public GameObject bulletPrefabUpgraded;

    public VulcanCannonSettings settings;
    ReloaderBehavior _reloader;
    public string CurrentState => _state?.Name;

    private int _currentBarrel = 0;
    public float bulletSpeed = 10.0f;
    public float damagePerBullet = 10.0f;

    ObjectPool<GameObject> _bulletPool = null;

    static Dictionary<GameObject, ObjectPool<GameObject>> _bulletPoolDictionary = new Dictionary<GameObject, ObjectPool<GameObject>>();

    public float rotationSpeed = 180.0f;

    public float BulletsPerSecond = 10.0f;


    // Start is called before the first frame update
    void Start()
    {
        if(!_bulletPoolDictionary.ContainsKey(bulletPrefab))
        {
            _bulletPoolDictionary[bulletPrefab] = new ObjectPool<GameObject>(
               () => Instantiate(bulletPrefab, WeaponsContainerBehavior.Instance.transform), // Create
               x => x.SetActive(true), // Take
               x => x.SetActive(false), // Return to bool
               x => Destroy(x), // Destroy
               true, // Collection checks
               10, // Default size
               500); // Max Size
        }
        if (bulletPrefabUpgraded != null && !_bulletPoolDictionary.ContainsKey(bulletPrefabUpgraded))
        {
            _bulletPoolDictionary[bulletPrefabUpgraded] = new ObjectPool<GameObject>(
               () => Instantiate(bulletPrefabUpgraded, WeaponsContainerBehavior.Instance.transform), // Create
               x => x.SetActive(true), // Take
               x => x.SetActive(false), // Return to bool
               x => Destroy(x), // Destroy
               true, // Collection checks
               10, // Default size
               500); // Max Size
        }

        _bulletPool = _bulletPoolDictionary[bulletPrefab];

        _state = new StateManager<VulcanCannonBehavior>(new VulcanCannonIdleState(this));

        _reloader = GetComponent<ReloaderBehavior>();
        _reloader.reloadTime = settings.reloadTime;
    }

    public void OnUpgradeUnlocked()
    {
        _state = new StateManager<VulcanCannonBehavior>(new VulcanCannonIdleState(this));
    }

    public void UpgradeSecondTurret()
    {
        if (GetComponent<UpgradableBehavior>().activeUpgrades.Contains(Upgrades.VULCAN_CANNON_3))
            return;

        SetBulletsPerSecond(5);
        OnUpgradeUnlocked();
    }

    public void UpgradeThirdTurret()
    {
        SetBulletsPerSecond(10f);
        OnUpgradeUnlocked();
    }

    public void UpgradeRadar()
    {
        this.GetComponent<AutoAimBehavior>().recalculateAimTime = 0.2f;
        this.GetComponent<AutoAimBehavior>().range *= 1.5f;

        OnUpgradeUnlocked();
    }

    public void UpgradeAmmoBox()
    {
        SetBulletsPerSecond(this.BulletsPerSecond);
        OnUpgradeUnlocked();
    }

    public void UpgradeAmmo()
    {
        _bulletPool = _bulletPoolDictionary[bulletPrefabUpgraded];
        this.bulletSpeed *= 1.5f;
        this.damagePerBullet *= 1.25f;
        OnUpgradeUnlocked();
    }

    void SetBulletsPerSecond(float bulletsPerSecond)
    {
        this.BulletsPerSecond = bulletsPerSecond;
        if (GetComponent<UpgradableBehavior>().activeUpgrades.Contains(Upgrades.VULCAN_AMMO))
            this.BulletsPerSecond *= 1.5f;
    }

    private void OnDestroy()
    {
        _state.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {
        _state.Update();
    }

    public void Fire()
    {
        var target = GetComponent<AutoAimBehavior>().currentTarget;
        if (_reloader.Ammo < 1.0 || target == null)
            return;

        float lifeTime = GetComponent<AutoAimBehavior>().range / bulletSpeed;

        var bullet = this._bulletPool.Get();
        var bulletBehavior = bullet.GetComponent<RailGunBulletBehavior>();
        bulletBehavior.Speed = bulletSpeed;
        bulletBehavior.Damage = damagePerBullet;
        bulletBehavior.LifeTime = lifeTime;
        bulletBehavior.Pool = this._bulletPool;

        Vector3 firePosition = this.transform.position;

        for(int i = 0; i < cannonTips.Count; i++)
        {
            int barrel = _currentBarrel++ % cannonTips.Count;
            var barrelTip = cannonTips[barrel];
            firePosition = barrelTip.position;
            if (barrelTip.gameObject.activeInHierarchy)
            {
                barrelTip.GetComponentInChildren<ParticleSystem>().Play();
                break;
            }
        }

        var delta = target.transform.position - firePosition;
        Vector3 targetPosition = target.transform.position;
        var targetRigidBody = target.GetComponent<Rigidbody2D>();
        if (targetRigidBody != null)
        {
            float reachTime = delta.magnitude / (bulletSpeed * 2.0f);
            var targetTravelDistance = (targetRigidBody.velocity) * reachTime;
            targetPosition = targetPosition + new Vector3(targetTravelDistance.x, targetTravelDistance.y, target.transform.position.z);
        }
        delta = targetPosition - firePosition;

        float angle = Vector2.SignedAngle(Vector2.left, delta);

        bullet.transform.position = firePosition;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90.0f);
        bulletBehavior.Fire();

        _reloader.Ammo--;
    }
}
