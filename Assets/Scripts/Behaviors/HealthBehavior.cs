using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnDamageEvent : UnityEvent<float>
{

}

[Serializable]
public class OnHealthChanged : UnityEvent<float, float>
{

}

public class HealthBehavior : MonoBehaviour, IDamagable
{
    public float MaxHealth = 100.0f;
    public float _health = 100.0f;
    public bool DestroyOnDeath = true;

    public OnHealthChanged OnHealthChanged;

    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            if (_health == value)
                return;

            _health = Mathf.Clamp(value, 0, MaxHealth);
            OnHealthChanged?.Invoke(_health, MaxHealth);

            if (Health <= 0)
            {
                if (DestroyOnDeath)
                    Destroy(this.gameObject);
            }
        }
    }

    void Start()
    {
        Health = MaxHealth;
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

    public void DoDamage(float damage)
    {
        Health = Mathf.Clamp(Health - damage, 0.0f, MaxHealth);
    }

    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
        Health = maxHealth;
    }
}
