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

    StateManager<RailGunBehavior> _state = null;

    public string CurrentState => _state?.Name;

    public bool CanFire = false;
    Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        var initialState = new RailGunIdleState(this);
        _state = new StateManager<RailGunBehavior>(initialState);
        _animator = this.GetComponent<Animator>();
        ChargeUp();
    }

    // Update is called once per frame
    void Update()
    {
        _state.Update();
    }

    public void ChargeUp()
    {
        _animator.Play("rail_gun_charging_animation");
    }

    public bool IsCharged = false;
    //To be called by the animator
    public void ChargeUpStarted()
    {
        // Started charging up - disable firing
        IsCharged = false;
        CanFire = false;
        if (chargingParticleSystem != null)
            chargingParticleSystem.Play();
    }

    // Called by recoil animation
    public void EnableCanFire()
    {
        CanFire = true;

        // Start charging up immediatly
        ChargeUp();
    }

    public void ChargeUpCompleted()
    {
        if (chargingParticleSystem != null)
            chargingParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        IsCharged = true;
        if (this.GetComponent<AutoAimBehavior>().currentTarget != null)
        {
            ReleaseBullet();
        }
        else
        {
            CanFire = true;
        }
    }

    public void ReleaseBullet()
    {
        IsCharged = false;

        _animator.Play("railgun_shooting_animation");
        this.GetComponent<AudioSource>().Play();

        var bullet = Instantiate(bulletPrefab, WeaponsContainerBehavior.Instance.transform);
        bullet.transform.position = railGunTip.position;
        bullet.transform.rotation = railGunTip.rotation;
        bullet.GetComponent<RailGunBulletBehavior>().Speed = bulletSpeed;
        bullet.GetComponent<RailGunBulletBehavior>().Damage = damage;
        bullet.GetComponent<RailGunBulletBehavior>().Fire();

        if (muzzleFlash != null)
            muzzleFlash.Play();
    }


    public void CancelFire()
    {
        if (chargingParticleSystem != null)
            chargingParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        _animator.SetTrigger("cancel_fire");
    }

    public void Fire()
    {
        if (!CanFire)
            return;

        if (this.GetComponent<AutoAimBehavior>().currentTarget == null)
            return;

        if (!IsCharged)
            ChargeUp();
        else
            ReleaseBullet();
    }
}
