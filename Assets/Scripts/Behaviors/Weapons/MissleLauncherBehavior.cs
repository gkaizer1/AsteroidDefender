using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MissleLauncherFiringState : IState<MissleLauncherBehavior>
{
    public override string Name => "Firing";

    GameObject _target = null;

    float fireCountDown = 0.0f;

    public MissleLauncherFiringState(MissleLauncherBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _target = Parent.GetComponent<AutoAimBehavior>().currentTarget;
    }

    public override IState<MissleLauncherBehavior> Update()
    {
        if (_target == null)
        {
            return new MissleLauncherRotateToTargetState(Parent);
        }

        Vector2 delta = _target.transform.position - Parent.missleLauncher.transform.position;

        // Keep top rotated towards target
        {
            float angle = Vector2.SignedAngle(Vector2.up, delta);
            float angleDelta = Mathf.DeltaAngle(Parent.missleLauncher.transform.rotation.eulerAngles.z, angle);
            float anglePerStep = Parent.RotationAnglePerSecond * Time.deltaTime;
            Parent.missleLauncher.transform.rotation = Quaternion.Euler(
                0f,
                0f,
                Parent.missleLauncher.transform.rotation.eulerAngles.z + angleDelta);
        }

        fireCountDown -= Time.deltaTime;
        if (fireCountDown <= 0)
        {
            fireCountDown = Parent.secondsPerMissle;
            Parent.Fire();
        }

        return this;
    }
}

public class MissleLauncherRotateToTargetState : IState<MissleLauncherBehavior>
{
    public override string Name => "Acquiring Target";

    AutoAimBehavior _autoAimBehavior = null;
    float coolDownTimer = 2.0f;
    float coolDownTime = 2.0f;

    public MissleLauncherRotateToTargetState(MissleLauncherBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
    }

    public override IState<MissleLauncherBehavior> Update()
    {
        var target = _autoAimBehavior.currentTarget;
        if (target == null)
        {
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer < 0)
                return new MissleLauncherIdleState(Parent);
            else
                return this;
        }

        // Reset the cooldown
        coolDownTimer = coolDownTime;

        Vector2 delta = target.transform.position - Parent.missleLauncher.transform.position;
        float angleFromNormal = Vector2.SignedAngle(Vector2.up, delta);
        float angleDelta = Mathf.DeltaAngle(Parent.missleLauncher.transform.rotation.eulerAngles.z, angleFromNormal);

        float anglePerStep = Parent.RotationAnglePerSecond * Time.deltaTime;

        float rotation = Mathf.Clamp(angleDelta, -anglePerStep, anglePerStep);
        Parent.missleLauncher.transform.rotation = Quaternion.Euler(
            0f, 
            0f, 
            Parent.missleLauncher.transform.rotation.eulerAngles.z + rotation);

        if (Mathf.Abs(angleDelta) < 0.2f)
        {
            return new MissleLauncherFiringState(Parent);

        }

        return this;
    }
}

public class MissleLauncherIdleState : IState<MissleLauncherBehavior>
{
    public override string Name => "Idle";

    AutoAimBehavior _autoAimBehavior = null;
    RotatorBehavior _rotator = null;

    public MissleLauncherIdleState(MissleLauncherBehavior obj) : base(obj)
    {
    }

    public override void Init()
    {
        _autoAimBehavior = Parent.GetComponent<AutoAimBehavior>();
        _rotator = Parent.missleLauncher.GetComponent<RotatorBehavior>();
        _rotator.enabled = true;
    }

    public override IState<MissleLauncherBehavior> Update()
    {
        if (_autoAimBehavior.currentTarget != null)
        {
            _rotator.enabled = false;
            return new MissleLauncherRotateToTargetState(Parent);
        }

        return this;
    }
}

public class MissleBay
{
    public GameObject missle;
    public bool readyToFire = false;
    public bool isEmpty = true;
}

public class MissleLauncherBehavior : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject misslePrefab;

    [Header("Children")]
    public GameObject missleLauncher;
    public float RotationAnglePerSecond = 90.0f;
    public List<GameObject> missleSpawnPoints = new List<GameObject>();

    List<MissleBay> _missileBays = new List<MissleBay>();

    [Header("Fire Settings")]
    public float secondsPerMissle = 2.0f;

    IState<MissleLauncherBehavior> _state;

    ReloaderBehavior _reloader = null;

    int _previousFiredMissleBay = -1;

    public float MissleDamage = 50f;

    void Awake()
    {
        _reloader = GetComponent<ReloaderBehavior>();
        _state = new MissleLauncherIdleState(this);
        for (int i = 0; i < missleSpawnPoints.Count; i++)
            _missileBays.Add(new MissleBay());
    }

    // Update is called once per frame
    void Update()
    {
        _state = _state.Update();
    }

    public void Fire()
    {
        if (_reloader.Ammo < 1.0f)
            return;


        if (this.GetComponent<AutoAimBehavior>().currentTarget == null)
            return;

        int missleIndex = -1;

        // Check all missle bays that are ready to fire - starting with the previous (fired + 1)
        // to prevent firing from the same missle bay on repeat
        for (int i = 0; i < _missileBays.Count; i++)
        {
            _previousFiredMissleBay = (_previousFiredMissleBay + 1) % _missileBays.Count;
            if (_missileBays[_previousFiredMissleBay].readyToFire)
            {
                missleIndex = _previousFiredMissleBay;
                break;
            }
        }

        if (missleIndex < 0)
            return;

        var missle = _missileBays[missleIndex].missle;
        missle.transform.parent = null;
        missle.GetComponent<AutoAimBehavior>().currentTarget = this.GetComponent<AutoAimBehavior>().currentTarget;

        //UnityEngine.Events.UnityAction<GameObject> listener = (newTarget) =>
        //{
        //    missle.GetComponent<NukeBehavior>().SelfDestruct();
        //};
        //missle.GetComponent<AutoAimBehavior>().OnTargetChanged.AddListener(listener);

        missle.GetComponent<NukeBehavior>().Fire().OnExplosion((explosionPoint) =>
        {
            int[] physicsMask = new int[3]
            {
                1 << LayerMask.NameToLayer("Astroids"),
                1 << LayerMask.NameToLayer("astroids_no_collisions"),
                1 << LayerMask.NameToLayer("enemies"),
            };

            List<HealthBehavior> colliderObject = new List<HealthBehavior>();

            foreach (var mask in physicsMask)
            {
                var itemsInRadius = Physics2D.OverlapCircleAll(explosionPoint, 0.5f, mask);
                foreach (Collider2D collider in itemsInRadius)
                {
                    var healthBehavior = collider.GetComponent<HealthBehavior>();
                    if(healthBehavior != null)
                        healthBehavior.Health -= MissleDamage;                    
                }
            }
        });

        _missileBays[missleIndex].readyToFire = false;
        _missileBays[missleIndex].missle = null;

        // Empty out this missle bay in 1 second
        StartCoroutine(co_missleLaunched(missleIndex));
    }

    IEnumerator co_missleLaunched(int index)
    {
        yield return new WaitForSeconds(1);
        _missileBays[index].isEmpty = true;
        _reloader.Ammo--;
    }

    public void OnAmmoChanged(float current, float max)
    {
        int ammo = Mathf.FloorToInt(current);
        int readyMissleBays = _missileBays.FindAll(x => x.isEmpty == false).Count;
        if (ammo != readyMissleBays)
        {
            SpawnMissle();
        }
    }

    public void SpawnMissle()
    {
        int index = _missileBays.FindLastIndex(x => x.isEmpty);
        if (index >= missleSpawnPoints.Count)
            return;

        var missle = Instantiate(misslePrefab, missleSpawnPoints[index].transform);
        missle.transform.localRotation = Quaternion.Euler(0f, 0f, 0);

        missle.transform.localPosition = missle.transform.localPosition + (Vector3.down * 0.1f);
        _missileBays[index] = new MissleBay()
        {
            missle = missle,
            readyToFire = false,
            isEmpty = false
        };
        missle.transform.DOLocalMove(Vector3.zero, 1.0f).OnComplete(() =>
        {
            _missileBays[index].readyToFire = true;
        });
    }
}
