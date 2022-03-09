using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class WeaponSlot
{
    public bool UnderConstruction = false;
    public Transform transform;
    public WeaponBehavior equipedWeapon;
    private GameObject weaponInstance;

    public void AddWeapon(GameObject weaponPrefab)
    {
        if (UnderConstruction)
            return;

        UnderConstruction = true;

        // Remove the existing weapon first
        RemoveWeapon();

        ShuttleMission_UpgradeSatellite mission = new ShuttleMission_UpgradeSatellite(this.transform.gameObject, 
            new BuildUpgradeInfo()
            {
                OnBuildCompleted = () =>
                {
                    weaponInstance = GameObject.Instantiate(weaponPrefab, transform);
                    equipedWeapon = weaponInstance.GetComponent<WeaponBehavior>();
                    UnderConstruction = false;
                }
            });
        ShuttleMissionManager.Instance.ScheduleMission(mission);
    }

    public void RemoveWeapon()
    {
        if (weaponInstance != null)
        {
            GameObject.Destroy(weaponInstance);
            weaponInstance = null;
        }
    }
}

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class SatelliteBehavior : MonoBehaviour
{
    private Animator _animator;
    public static event System.Action OnSatelliteMoved;

    public GameObject turnRadiusIndicator;
    public DragableBehavior rotatorHandle;
    public float minAltitude = 11.0f;
    public float moveSpeed = 5.0f;
    public float bodyRotationAnglePerSecond = 90.0f;

    public GameObject endTurnIndicator;
    GameObject _endTurnIndicatorInstance;
    BoxCollider2D _boxCollider2D;

    public GameObject body;

    public GameObject movePrefabSatellite;
    GameObject _movePrefabInstance;

    PolarCoordinateTransform _polarTransform;

    float _dragRadiusEnd = 0.0f;
    float _dragAngleEnd = 0.0f;

    public UnityEvent<SatelliteBehavior> OnMoveStarted;
    public UnityEvent<SatelliteBehavior> OnMoveEnded;

    public bool IsRelocating = false;
    public bool CanMove = true;

    public List<WeaponSlot> WeaponSockets = new List<WeaponSlot>();

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

        _polarTransform = GetComponent<PolarCoordinateTransform>();
        if (_polarTransform == null)
            gameObject.UnassignedReference($"Satellite missing polar transform component");

        _boxCollider2D = GetComponent<BoxCollider2D>();
        if (_boxCollider2D == null)
            _boxCollider2D = GetComponentInParent<BoxCollider2D>();
        if (_boxCollider2D == null)
            gameObject.UnassignedReference($"Satellite missing polar transform component");

#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
#endif
            var container = GameObject.Find("SATELLITE_CONTAINER");
            if (container != null)
            {
                this.transform.parent = container.transform;
                if (turnRadiusIndicator != null)
                {
                    turnRadiusIndicator.transform.parent = container.transform;
                    turnRadiusIndicator.transform.position = Vector3.zero;
                }
            }
            if (turnRadiusIndicator != null)
            {
                turnRadiusIndicator.GetComponent<CircleBehavior>().radius = 0.0f;
                turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = 0;
                turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = 0;
                turnRadiusIndicator.SetActive(false);
            }

            if(!IsValidLocation(this._polarTransform.Radius, this._polarTransform.Angle))
                MoveToNextLegalLocation();
#if UNITY_EDITOR
        }
#endif
    }

    private void OnDestroy()
    {
        if (turnRadiusIndicator != null)
            Destroy(turnRadiusIndicator);
    }

    public void OnMouseDragStart(GameObject sender, Vector3 start)
    {
        _previousMoveValid = true;
        if (_movePrefabInstance != null)
        {
            Destroy(_movePrefabInstance);
            _movePrefabInstance = null;
        }
        
        _movePrefabInstance = GameObject.Instantiate(this.movePrefabSatellite, this.transform.parent);
        _movePrefabInstance.name = $"{this.name}_move";
        _movePrefabInstance.GetComponent<PolarCoordinateTransform>().Angle = _polarTransform.Angle;
        _movePrefabInstance.GetComponent<PolarCoordinateTransform>().Radius = _polarTransform.Radius;
        _movePrefabInstance.transform.rotation = Quaternion.Euler(0f, 0f, _polarTransform.Angle);

        SelectionManager.SelectedGameObject = this.gameObject;

        foreach (var autoAimBehavior in this.GetComponentsInChildren<AutoAimBehavior>())
            autoAimBehavior.ShowRangeCircle();

        _dragAngleEnd = _polarTransform.Angle;
        _dragRadiusEnd = _polarTransform.Radius;

        if (turnRadiusIndicator != null)
        {
            turnRadiusIndicator.SetActive(true);
            turnRadiusIndicator.GetComponent<CircleBehavior>().radius = _polarTransform.Radius;
            turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = _polarTransform.Angle;
            turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = _polarTransform.Angle;
        }
    }

    bool _previousMoveValid = true;

    public void OnMouseDrag(GameObject sender, Vector3 start, Vector3 end)
    {
        Doozy.Engine.GameEventMessage.SendEvent("HIDE_BUILD_PANELS");
        var endGameWorld = Camera.main.ScreenToWorldPoint(end);

        Vector3 currentCoords = Utils.PolarToCartesian(_dragAngleEnd, _dragRadiusEnd);
        var distanceFromEnd = Vector2.Distance(currentCoords, endGameWorld);

        Vector3 delta = endGameWorld - currentCoords;
        float vertAngle = Vector2.SignedAngle(Utils.PolarToCartesian(_dragAngleEnd, 1), delta);

        // Mouse is pointing "up"
        if (Mathf.Abs(vertAngle) < 10.0f)
        {
            if (distanceFromEnd > 2.0f && _dragRadiusEnd < (_polarTransform.Radius + 10))
                _dragRadiusEnd += 2;
        }
        if (Mathf.Abs(vertAngle) > 170.0f)
        {
            if (distanceFromEnd > 2.0f && _dragRadiusEnd > (_polarTransform.Radius - 10))
                _dragRadiusEnd -= 2;
        }

        _dragRadiusEnd = Mathf.Floor(_dragRadiusEnd);

        if (_dragRadiusEnd < minAltitude)
            _dragRadiusEnd = minAltitude;

        float angleDelta = Vector2.SignedAngle(
                                Camera.main.ScreenToWorldPoint(start),
                                Camera.main.ScreenToWorldPoint(end));

        _dragAngleEnd = AngleToNearestValid(_polarTransform.Angle + angleDelta, _dragRadiusEnd);

        turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = _polarTransform.Angle;
        turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = _dragAngleEnd;
        turnRadiusIndicator.GetComponent<CircleBehavior>().radius = _dragRadiusEnd;
        turnRadiusIndicator.GetComponent<CircleBehavior>().startRadius = _polarTransform.Radius;

        _movePrefabInstance.GetComponent<PolarCoordinateTransform>().Radius = _dragRadiusEnd;
        _movePrefabInstance.GetComponent<PolarCoordinateTransform>().Angle = _dragAngleEnd;
        _movePrefabInstance.transform.rotation = Quaternion.Euler(0f, 0f, _dragAngleEnd);

        _movePrefabInstance.transform.rotation = Quaternion.Euler(0f, 0f, _dragAngleEnd);
        _movePrefabInstance.transform.localRotation = Quaternion.Euler(0f, 0f, _dragAngleEnd);
        if (!IsValidLocation(_dragRadiusEnd, _dragAngleEnd))
        {
            if (_previousMoveValid)
            {
                var children = _movePrefabInstance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var child in children)
                {
                    child?.material.SetColor("_Color", Color.red);
                }
                _previousMoveValid = false;
            }
        }
        else
        {
            if(!_previousMoveValid)
            {
                _previousMoveValid = true;
                var children = _movePrefabInstance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var child in children)
                {
                    child?.material.SetColor("_Color", Color.white);
                }
            }
        }

    }

    public void OnMouseDragEnd(GameObject sender, Vector3 start, Vector3 end)
    {
        foreach (var autoAimBehavior in this.GetComponentsInChildren<AutoAimBehavior>())
            autoAimBehavior.HideRangeCircle();

        // Cleanup move prefab indicator
        if (_movePrefabInstance != null)
        {
            Destroy(_movePrefabInstance);
            _movePrefabInstance = null;
        }

        if (!IsValidLocation(_dragRadiusEnd, _dragAngleEnd))
        {
            turnRadiusIndicator.SetActive(false);
            return;
        }

        // Remove the turn radius Indicator
        turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = 0;
        turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = 0;
        turnRadiusIndicator.SetActive(false);

        // Delesect this object
        SelectionManager.SelectedGameObject = null;

        MoveSatellite(_dragRadiusEnd, _dragAngleEnd);
    }

    public float AngleToNearestValid(float angle, float radius)
    {
        // Round down radius in increment of 5 (e.g radis 17 -> 15, 23 -> 20, etc.)
        radius = radius - (radius % 7.0f);
        float degPerGameUnit = 360.0f / (2.0f * radius * Mathf.PI);
        float degPerSatellite = 0.5f * degPerGameUnit;

        // Get an integer multiplier
        return Mathf.Round(angle / degPerSatellite) * degPerSatellite;
    }

    public float NextAngleToNearestValid(float angle, float count)
    {
        // Round down radius in increment of 5 (e.g radis 17 -> 15, 23 -> 20, etc.)
        float radius = _polarTransform.Radius - (_polarTransform.Radius % 7.0f);
        float degPerGameUnit = 360.0f / (2.0f * radius * Mathf.PI);
        float degPerSatellite = 0.5f * degPerGameUnit;
        float offset = degPerSatellite * count;

        // Get an integer multiplier
        return Mathf.Round((angle + offset) / degPerSatellite) * (degPerSatellite);
    }

    System.Action _onExhaustChanged;
    bool isExhuastOn = false;
    public void ToggleExhaust(bool exhuastOn, System.Action onCompleted = null)
    {
        if (isExhuastOn == exhuastOn)
            return;

        isExhuastOn = exhuastOn;
        _animator?.Play(exhuastOn ? "exhaust_on" : "exhaust_off");
        _onExhaustChanged = onCompleted;
    }

    public void OnExhaustedChanged()
    {
        _onExhaustChanged?.Invoke();
        _onExhaustChanged = null;
    }

    Vector3 _nextCheckpoint = Vector3.zero;
    public void MoveSatellite(float endRadius, float endAngle, System.Action OnMoveCompleted = null)
    {
        if (this.IsRelocating || !this.CanMove)
            return;

        this.IsRelocating = true;
        OnMoveStarted?.Invoke(this);

        endAngle = AngleToNearestValid(endAngle, endRadius);

        if (_endTurnIndicatorInstance != null)
        {
            Destroy(_endTurnIndicatorInstance);
            _endTurnIndicatorInstance = null;
        }

        // add the final point
        if (_endTurnIndicatorInstance == null)            
            _endTurnIndicatorInstance = Instantiate(this.endTurnIndicator, this.transform.parent);            

        _endTurnIndicatorInstance.transform.position = Utils.PolarToCartesian(endAngle, endRadius);
        _endTurnIndicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, endAngle);
        _endTurnIndicatorInstance.transform.localRotation = Quaternion.Euler(0f, 0f, 180.0f + endAngle);        


        if (turnRadiusIndicator)
        {
            turnRadiusIndicator.GetComponent<CircleBehavior>().radius = endRadius;
            turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = _polarTransform.Angle;
            turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = endAngle;
            turnRadiusIndicator.SetActive(true);
        }

        float rotationDelta = endAngle - _polarTransform.Angle;
        float radiusDelta = endRadius - _polarTransform.Radius;

        float distance = (endRadius * 2f * Mathf.PI) * (Mathf.Abs(rotationDelta) / 360f);
        float rotationTime = (distance / moveSpeed) + Mathf.Abs(radiusDelta);

        // 1. Rotate the body to the right orientation
        // 2. Start moving !
        float rotationDirection = rotationDelta <= 0 ? 180f : 0f;
        float angleDelta = Mathf.DeltaAngle(this.body.transform.rotation.eulerAngles.z, _polarTransform.Angle + rotationDirection);
        float timeToRotate = Mathf.Abs(angleDelta / 180.0f * 2.0f) + (radiusDelta * 2.0f);

        _nextCheckpoint = Utils.PolarToCartesian(
            this._polarTransform.Angle + (0.1f * rotationDelta), 
            this._polarTransform.Radius + (0.1f * radiusDelta));
        float bodyAngleDelta = Vector3.SignedAngle(this.transform.up, this.transform.position - _nextCheckpoint, Vector3.forward);

        float bodyRotationDelta = Mathf.DeltaAngle(this.body.transform.rotation.eulerAngles.z, bodyAngleDelta);
        float bodyRotationTime = Mathf.Abs(bodyRotationDelta) / bodyRotationAnglePerSecond;
        this.body.transform.DORotate(new Vector3(0, 0, bodyAngleDelta), bodyRotationTime)
            .OnStart(() =>
            {
                turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = _polarTransform.Angle;
                turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = endAngle;

                turnRadiusIndicator.GetComponent<CircleBehavior>().startRadius = _polarTransform.Radius;
                turnRadiusIndicator.GetComponent<CircleBehavior>().radius = endRadius;
                turnRadiusIndicator.SetActive(true);
                ToggleExhaust(false);
            })
            .OnComplete(() =>
            {
                ToggleExhaust(true, 
                    // After exhaust has been turned on
                    // Only then start moving
                    () =>
                    {
                        float currentPrecentage = 0.0f;
                        float startAngle = _polarTransform.Angle;
                        float startRadius = _polarTransform.Radius;

                        DOTween.To( () => currentPrecentage,
                                    x => currentPrecentage = x,
                                    1.0f,
                                    rotationTime)
                        .OnUpdate(() =>
                        {
                            _polarTransform.Angle = startAngle + (currentPrecentage * rotationDelta);
                            _polarTransform.Radius = startRadius + (currentPrecentage * radiusDelta);
                            turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = _polarTransform.Angle;
                            turnRadiusIndicator.GetComponent<CircleBehavior>().startRadius = _polarTransform.Radius;

                            // Keep adjusting the body to the next checkpoint
                            _nextCheckpoint = Utils.PolarToCartesian(
                                this._polarTransform.Angle + (0.1f * rotationDelta),
                                this._polarTransform.Radius + (0.1f * radiusDelta));
                            float bodyAngleDelta = Vector3.SignedAngle(this.transform.up, this.transform.position - _nextCheckpoint, Vector3.forward);
                            this.body.transform.rotation = Quaternion.Euler(0, 0, bodyAngleDelta);
                        })
                        .OnStart(() =>
                        {
                        })
                        .OnComplete(() =>
                        {
                            // Disabled - since the body should be in the correct position by this point
                            // Reset rotation to proper horizontal angle
                            // body.transform.localRotation = Quaternion.Euler(0, 0, 0.0f);

                            OnMoved();
                            OnMoveCompleted?.Invoke();
                        });
                    });
            });
    }

    void RotateToStationary(System.Action onRotationCompelted = null)
    {
        float currentRotation = this.body.transform.rotation.eulerAngles.z;
        float finalAngle = this._polarTransform.Angle;
        float bodyRotationDelta = Mathf.DeltaAngle(this.body.transform.rotation.eulerAngles.z, finalAngle);
        float bodyRotationTime = Mathf.Abs(bodyRotationDelta) / bodyRotationAnglePerSecond;
        this.body.transform.DOLocalRotate(new Vector3(0, 0, finalAngle), bodyRotationTime).OnComplete(() =>
        {
            onRotationCompelted?.Invoke();
            OnMoveEnded?.Invoke(this);
        });
    }

    void OnMoved()
    {
        if (_endTurnIndicatorInstance != null)
        {
            Destroy(_endTurnIndicatorInstance);
            _endTurnIndicatorInstance = null;
        }

        RotateToStationary(() =>
        {
            this.IsRelocating = false;
            OnSatelliteMoved?.Invoke();
        });

        if (turnRadiusIndicator != null)
            turnRadiusIndicator.SetActive(false);

        ToggleExhaust(false);

        if (!IsValidLocation(_polarTransform.Radius, _polarTransform.Angle))
            MoveToNextLegalLocation();
    }

    static bool _canMoveNextSatellite = true;
    private IEnumerator co_MoveToNextLegalLocation()
    {
        // Quick explanation
        // We only want to schedule ONE move at a time
        // so only one satellite can escape this loop at a time

        while(!_canMoveNextSatellite)
        {
            yield return new WaitForSeconds(0.25f);
        }

        float minRadius = this._polarTransform.Radius;
        if (this._polarTransform.Radius < minAltitude)
            minRadius = minAltitude;

        for (int r = 0; r < 10; r++)
        {
            bool done = false;
            float radius = minRadius + r;
            for (float i = 0; i < 180f && !done; i += 5f)
            {
                float delta = (i % 180f);
                float newAngle = this._polarTransform.Angle + delta;
                float nextAngle = AngleToNearestValid(newAngle, minRadius + radius);
                if (IsValidLocation(radius, nextAngle))
                {
                    _canMoveNextSatellite = false;
                    MoveSatellite(radius, nextAngle);
                    done = true;
                    break;
                }

                newAngle = this._polarTransform.Angle - delta;
                nextAngle = AngleToNearestValid(newAngle, minRadius + radius);
                if (IsValidLocation(radius, nextAngle))
                {
                    _canMoveNextSatellite = false;
                    MoveSatellite(radius, nextAngle);
                    done = true;
                    break;
                }
            }
            if(done)
            {
                break;
            }
        }

        yield return new WaitForSeconds(0.25f);
        _canMoveNextSatellite = true;

        yield return null;
    }

    private void MoveToNextLegalLocation()
    {
        StartCoroutine(co_MoveToNextLegalLocation());
    }

    public bool IsValidLocation(float radius, float angle)
    {
        var satelliteLayer = LayerMask.NameToLayer("satellites");
        var collisions = Physics2D.OverlapBoxAll(Utils.PolarToCartesian(angle, radius), new Vector3(1, 1, 1), 0f, 1 << satelliteLayer);
        foreach (var collision in collisions)
        {
            var satellite = collision.gameObject.GetComponent<SatelliteBehavior>();
            if (satellite?.IsRelocating ?? false)
                continue;

            if (_movePrefabInstance && collision.gameObject == _movePrefabInstance)
                continue;

            if (_endTurnIndicatorInstance && collision.gameObject == _endTurnIndicatorInstance)
                continue;

            if (collision.gameObject == this.gameObject)
                continue;

            return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {        
        Gizmos.DrawCube(_nextCheckpoint, new Vector3(0.2f, 0.2f, 1));
        //for (int i = 0; i < 360; i += 10)
        //{
        //    float angle = AngleToNearestValid(i, this._polarTransform.Radius);
        //    Gizmos.DrawCube(Utils.PolarToCartesian(angle, this._polarTransform.Radius), new Vector3(1, 1, 1));
        //}
    }

#if UNITY_EDITOR
    [Header("Editor ONLY")]
    public bool _MOVE_TO_NEAREST = false;
    public void OnValidate()
    {
        if (_MOVE_TO_NEAREST)
        {
            _MOVE_TO_NEAREST = false;
            float angle = AngleToNearestValid(this._polarTransform.Angle, this._polarTransform.Radius);
            this._polarTransform.Angle = angle;

            float finalAngle = this._polarTransform.Angle;
            this.body.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, finalAngle));
        }        
    }
#endif
}
