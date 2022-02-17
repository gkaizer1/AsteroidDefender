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
    public GameObject explodeOnDeathPrefab;

    private static GameObject _explosionContainer;

    public void Start()
    {
        // Try to resolve the explosion container if possible
        if(_explosionContainer == null)
        {
            _explosionContainer = GameObject.Find("ENEMIES_CONTAINER");
        }
        Health = MaxHealth;
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

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

    public void Explode()
    {
        if (explodeOnDeathPrefab != null)
        {
            var explosion = Instantiate(explodeOnDeathPrefab, _explosionContainer != null ? _explosionContainer.transform : this.transform.parent);
            explosion.name = $"explosion_{name}";
            explosion.transform.parent = this.transform.parent;
            explosion.transform.position = this.transform.position;
        }
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
