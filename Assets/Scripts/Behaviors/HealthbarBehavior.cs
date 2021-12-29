using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;

[Serializable]
public class HealthColorInformation
{
    public float HealthPrecentage = 1.0f;
    public Color color = Color.red;
    public string animationTrigger;
}

public class HealthbarBehavior : MonoBehaviour
{
    [SerializeField]
    private Image healthbarFill;

    private float _previousHealth = 100.0f;

    public List<HealthColorInformation> healthColor = new List<HealthColorInformation>();

    Tween _HealthTween = null;
    public Animator _animator;

    public void OnHealthChanged(float currentHealth, float maxHealth)
    {
        float fillPrecentage = currentHealth / maxHealth;
        float healthDelta = Mathf.Abs(_previousHealth - currentHealth);
        float maxAnimationTime = 3.0f;

        float healthPrecentage = healthDelta / maxHealth;
        float animationTime = Mathf.Clamp(maxAnimationTime * healthPrecentage, 0.5f, maxAnimationTime);

        if (_HealthTween != null)
        {
            _HealthTween.Kill();
        }

        foreach(var color in healthColor)
        {
            if((currentHealth/ maxHealth) < color.HealthPrecentage)
            {
                healthbarFill.color = color.color;
                if (_animator != null && !string.IsNullOrEmpty(color.animationTrigger))
                {
                    foreach (var trigger in _animator.parameters.Where(x => x.type == AnimatorControllerParameterType.Trigger))
                        _animator.ResetTrigger(trigger.name);
                    _animator.SetTrigger(color.animationTrigger);
                }

            }
        }

        _HealthTween = healthbarFill
            .DOFillAmount(fillPrecentage, animationTime)
            .SetUpdate(true)
            .OnComplete(() => {
            _HealthTween = null;
            _previousHealth = currentHealth;
        });
    }
}
