using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class RailGunBehavior : MonoBehaviour
{

    public GameObject bulletPrefab;

    public GameObject targetIndicatorPrefab;

    public GameObject railGunTop;

    public ParticleSystem muzzleFlash;

    public ParticleSystem chargingParticleSystem;

    public Transform railGunTip;

    public float RotationAnglePerSecond = 90.0f;

    public float bulletSpeed = 20.0f;
    public float damage = 100.0f;
    public float powerRequiredToFire = 5.0f;

    ReloaderBehavior _reloader = null;

    StateManager<RailGunBehavior> _state = null;

    public string CurrentState => _state?.Name;

    public bool CanFire = false;
    PowerConsumer _powerConsumer = null;
    Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        var initialState = new PowerManagerState<RailGunBehavior>(this, new RailGunIdleState(this))
                                            .OnOutPowerPower(() => new RailGunOutOfPowerState(this))
                                            .OnPowerRestored(() => new RailGunIdleState(this));

        _state = new StateManager<RailGunBehavior>(initialState);

        _reloader = GetComponent<ReloaderBehavior>();
        _powerConsumer = GetComponent<PowerConsumer>();
        _animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _state.Update();
    }

    public void OnAutoFire()
    {
        ChargeUp();
    }

    public void ChargeUp()
    {
        if (!CanFire)
            return;

        if (_reloader.Ammo < 1.0 || powerRequiredToFire > _powerConsumer.CurrentPower)
            return;

        if (chargingParticleSystem != null)
            chargingParticleSystem.Play();

        _animator.SetTrigger("fire");
        _animator.ResetTrigger("cancel_fire");

        CanFire = false;
    }

    public void CancelFire()
    {
        if (chargingParticleSystem != null)
            chargingParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        CanFire = true;
        _animator.SetTrigger("cancel_fire");
    }

    public void Fire()
    {
        if(chargingParticleSystem != null)
            chargingParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        CanFire = true;
        if (this.GetComponent<AutoAimBehavior>().currentTarget == null)
            return;

        this.GetComponent<AudioSource>().Play();
        var bullet = Instantiate(bulletPrefab, WeaponsContainerBehavior.Instance.transform);
        bullet.GetComponent<RailGunBulletBehavior>().Speed = bulletSpeed;
        bullet.GetComponent<RailGunBulletBehavior>().Damage = damage;
        bullet.transform.position = railGunTip.position;
        bullet.transform.rotation = railGunTip.rotation;

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        _reloader.Ammo--;
        _powerConsumer.CurrentPower -= powerRequiredToFire;
    }

    public void OnReloadChanged(float ammo, float maxAmmo)
    {
        if (ammo > 1.0f)
        {
            ChargeUp();
        }
    }
}
