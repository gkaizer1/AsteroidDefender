using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnGateCircleBehavior : MonoBehaviour
{
    [Header("Children")]
    public CircleBehavior spawnIndicator = null;
    public ParticleSystem spawnIndicatorParticleSystem = null;

    [Header("Prefabs")]
    public GameObject enemyMaskPrefab = null;

    float _radius = 20.0f;
    float _minAngle = 0.0f;
    float _maxAngle = 360.0f;
    GameObject _mask = null;

    [Header("Events")]
    public UnityEvent OnGateOpened = null;

    void Start()
    {
    }

    public void OpenSpawnGate(float minAngle, float maxAngle, float radius)
    {
        _radius = radius;
        _maxAngle = maxAngle;
        _minAngle = minAngle;

        _mask = Instantiate(enemyMaskPrefab, this.transform);
        _mask.transform.localScale = Vector3.one * (radius * 2.0f + 0.4f);

        float averageAngle = _minAngle + (_maxAngle - _minAngle) / 2f;
        float angleArc = (this._maxAngle - this._minAngle);

        float x = Mathf.Cos(averageAngle * Mathf.Deg2Rad) * (radius + 1);
        float y = Mathf.Sin(averageAngle * Mathf.Deg2Rad) * (radius + 1);

        if (spawnIndicatorParticleSystem != null)
        {
            var particleShape = spawnIndicatorParticleSystem.shape;
            particleShape.radius = radius;
            particleShape.arc = angleArc;
            spawnIndicatorParticleSystem.transform.rotation = Quaternion.Euler(0f, 0f, this._minAngle);
        }

        if (spawnIndicator != null)
        {
            spawnIndicator.MinAngle = averageAngle;
            spawnIndicator.MaxAngle = averageAngle;
            spawnIndicator.radius = radius;
            DG.Tweening.DOTween.To(
                () => spawnIndicator.MinAngle,
                x => spawnIndicator.MinAngle = x,
                this._minAngle,
                5f
            );
            Tween tween = DG.Tweening.DOTween.To(
                 () => spawnIndicator.MaxAngle,
                 x => spawnIndicator.MaxAngle = x,
                 this._maxAngle,
                 5f
             );
            tween.OnComplete(() =>
            {
                if (spawnIndicatorParticleSystem != null)
                {
                    spawnIndicatorParticleSystem.Play();
                }
                OnGateOpened?.Invoke();
            });
        }
    }

    private void OnDestroy()
    {
        if(_mask != null)
            Destroy(_mask);
    }

    public void CloseSpawnGate()
    {
        spawnIndicatorParticleSystem.Stop();
        float averageAngle = spawnIndicator.MinAngle + (spawnIndicator.MaxAngle - spawnIndicator.MinAngle) / 2f;
        DOTween.To(
            () => spawnIndicator.MinAngle,
            x => spawnIndicator.MinAngle = x,
            averageAngle,
            2f
        );
        DOTween.To(
            () => spawnIndicator.MaxAngle,
            x => spawnIndicator.MaxAngle = x,
            averageAngle,
            2f
        ).OnComplete(() =>
        {
            var lineRenderer = this.GetComponent<LineRenderer>();
            if (lineRenderer != null)
                lineRenderer.enabled = false;
        });
    }
}
