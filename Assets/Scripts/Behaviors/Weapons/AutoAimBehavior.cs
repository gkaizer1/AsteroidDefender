using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[Serializable]
public class AutoAimChangedEvent : UnityEvent<GameObject>
{
}

public class AutoAimBehavior : MonoBehaviour
{
    public GameObject currentTarget;

    public AutoAimChangedEvent OnTargetChanged;

    public float recalculateAimTime = 1.0f;

    [SerializeField]
    private GameObject targetIndicatorPrefab;
    GameObject _rangeCircle;

    public GameObject lineObjectPrefab;
    public float range = 10.0f;

    public enum TargetMode
    {
        RANDOM,
        CLOSEST,
        MIN_ROTATION
    };

    public GameObject rotationAnchor;

    public TargetMode mode = TargetMode.RANDOM;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTargetRoutine());
        SelectionManager.OnSelectedObjectChanged += OnSelectionChanged;
    }

    public void OnDestroy()
    {
        SelectionManager.OnSelectedObjectChanged -= OnSelectionChanged;
    }

    public void ShowRangeCircle()
    {
        if (_rangeCircle == null)
        {
            _rangeCircle = Instantiate(Resources.Load<GameObject>("AutoAim/auto_aim_circle"), this.transform);
            _rangeCircle.transform.parent = this.transform;
            _rangeCircle.transform.position = this.transform.position;
            var circleBehavior = _rangeCircle.GetComponent<CircleBehavior>();
            circleBehavior.radius = 0f;
            DG.Tweening.DOTween.To(
                () => circleBehavior.radius,
                x => circleBehavior.radius = x,
                range,
                0.5f);
        }
    }

    public void HideRangeCircle()
    {
        if (_rangeCircle != null)
        {
            Destroy(_rangeCircle);
            _rangeCircle = null;
        }
    }

    public void OnSelectionChanged(GameObject previous, GameObject current)
    {
        if (Utils.IsParent(this.gameObject, current))
        {
            ShowRangeCircle();
        }
        else
        {
            HideRangeCircle();
        }
    }

    public bool IsValidTarget(GameObject target)
    {
        if (target == null)
            return false;

        var detectable = target.GetComponent<DetectableBehavior>();
        if (detectable != null && detectable.IsDetected == false)
            return false;

        var enemyBehavior = target.GetComponent<EnemyBehavior>();
        if (enemyBehavior != null && enemyBehavior.IsActive == false)
            return false;

        var distance = (transform.position - target.transform.position).magnitude;
        if (distance > range)
            return false;

        return true;
    }

    public GameObject GetNextClosestTarget()
    {
        GameObject closestObject = null;
        float minDistance = float.MaxValue;

        var enemies = EnemyManager.Enemies;
        foreach(var enemy in enemies)
        {
            if (!IsValidTarget(enemy))
                continue;

            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance < minDistance)
            {
                closestObject = enemy;
                minDistance = distance;
            }
        }

        return closestObject;
    }
    public GameObject GetRandomTarget()
    {
        var enemies = EnemyManager.Enemies;
        List<GameObject> enemiesInRange = new List<GameObject>();
        foreach (var enemy in enemies)
        {
            if (!IsValidTarget(enemy))
                continue;

            enemiesInRange.Add(enemy);
        }
        if (enemiesInRange.Count <= 0)
            return null;

        int randomAstroid = Mathf.FloorToInt(UnityEngine.Random.Range(0, enemiesInRange.Count));
        return enemiesInRange[randomAstroid];
    }

    public GameObject GetMinRotation()
    {
        GameObject closestObject = null;
        float minAngle = float.MaxValue;

        var enemies = EnemyManager.Enemies;
        foreach (var enemy in enemies)
        {
            if (!IsValidTarget(enemy))
                continue;

            Vector2 delta = enemy.transform.position - rotationAnchor.transform.position;
            float angle = Mathf.Abs(Vector2.SignedAngle(rotationAnchor.transform.up, delta));

            if (angle < minAngle)
            {
                closestObject = enemy;
                minAngle = angle;
            }
        }

        return closestObject;
    }

    public void UpdateTarget()
    {
        GameObject newTarget = null;
        switch (mode)
        {
            case TargetMode.RANDOM:
                newTarget = GetRandomTarget();
                break;
            case TargetMode.CLOSEST:
                newTarget = GetNextClosestTarget();
                break;
            case TargetMode.MIN_ROTATION:
                newTarget = GetMinRotation();
                break;
        }


        if (currentTarget != newTarget)
        {
            currentTarget = newTarget;
            OnTargetChanged?.Invoke(currentTarget);
        }
        else
        {
            currentTarget = newTarget;
        }
    }

    IEnumerator UpdateTargetRoutine()
    {
        while (this.gameObject != null)
        {
            if (currentTarget != null)
            {
                if (!IsValidTarget(currentTarget))
                {
                    yield return new WaitForSeconds(recalculateAimTime);
                    currentTarget = null;
                    continue;
                }
            }

            if (currentTarget == null)
                UpdateTarget();

            yield return new WaitForSeconds(recalculateAimTime);
        }
    }
}
