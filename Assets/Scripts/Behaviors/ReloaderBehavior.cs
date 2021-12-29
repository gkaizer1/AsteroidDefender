using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AmmoChangedEvent : UnityEvent<float, float>
{
}

public class ReloaderBehavior : MonoBehaviour
{
    [SerializeField]
    private float _ammo = 0.0f;

    float _ammoInternal = 0.0f;

    public enum ReloadType
    {
        SINGLE,
        ALL_AT_ONCE
    };

    public ReloadType reloadType = ReloadType.SINGLE;

    public float Ammo
    {
        get
        {
            return _ammo;
        }
        set
        {
            if (_ammo == value)
                return;

            _ammoInternal = value;
            _ammo = value;
            onAmmoChangedCallBack?.Invoke(_ammo, maxAmmo);
        }
    }
    public float maxAmmo = 1.0f;
    public float reloadTime = 5.0f;

    [Header("Events")]
    public AmmoChangedEvent onAmmoChangedCallBack;

    [Header("Children")]
    public List<SpriteRenderer> fillableSprites = new List<SpriteRenderer>();

    void Start()
    {
        StartCoroutine(ReloadCoRoutine());
    }

    public IEnumerator ReloadCoRoutine()
    {
        var sleep = new WaitForSeconds(reloadTime);
        while (this.gameObject != null)
        {
            float secondsElapsed = (float)(reloadTime);

            if (Ammo >= maxAmmo)
                yield return new WaitForSeconds(reloadTime);

            _ammoInternal = Mathf.Clamp(_ammoInternal + (secondsElapsed * (1.0f / reloadTime)), 0.0f, maxAmmo);
            if (reloadType == ReloadType.SINGLE)
            {
                Ammo = Mathf.Clamp(_ammoInternal, 0.0f, maxAmmo);
            }
            else if (reloadType == ReloadType.ALL_AT_ONCE)
            {
                if (_ammoInternal == maxAmmo)
                    Ammo = maxAmmo;
            }

            // Update all items with fill materials
            fillableSprites.ForEach(x =>
            {
                if(x != null && x.material != null)
                    x.material.SetFloat("_FillPrecentage", Mathf.Clamp01(Ammo / maxAmmo));
            });

            yield return sleep;
        }
    }
}
