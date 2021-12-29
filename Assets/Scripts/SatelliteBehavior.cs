using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class SatelliteBehavior : MonoBehaviour
{
    public static event System.Action OnSatelliteMoved;

    public LineRenderer verticalLine;
    public GameObject turnRadiusIndicator;
    public DragableBehavior rotatorHandle;
    public float rotationSpeed = 5f;

    public GameObject endTurnIndicator;
    GameObject _endTurnIndicatorInstance;
    BoxCollider2D _boxCollider2D;

    public GameObject exhaust;
    public GameObject body;

    public GameObject movePrefabSatellite;
    GameObject _movePrefabInstance;

    PolarCoordinateTransform _polarTransform;

    float _dragRadiusEnd = 0.0f;
    float _dragAngleEnd = 0.0f;

    public UnityEvent<SatelliteBehavior> OnMoveStarted;
    public UnityEvent<SatelliteBehavior> OnMoveEnded;


    // Start is called before the first frame update
    void Start()
    {
        _polarTransform = GetComponent<PolarCoordinateTransform>();
        if (_polarTransform == null)
            gameObject.UnassignedReference($"Satellite missing polar transform component");

        _boxCollider2D = GetComponent<BoxCollider2D>();
        if (_boxCollider2D == null)
            _boxCollider2D = GetComponentInParent<BoxCollider2D>();

#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
#endif
            var container = GameObject.Find("SATELLITE_CONTAINER");
            if (container != null)
            {
                this.transform.parent = container.transform;
                if (turnRadiusIndicator != null)
                    turnRadiusIndicator.transform.parent = container.transform;
            }
#if UNITY_EDITOR
        }
#endif
        if (turnRadiusIndicator != null)
        {
            turnRadiusIndicator.transform.position = Vector3.zero;
            turnRadiusIndicator.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = 0;
            turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = 0;
        }
    }

    private void OnDestroy()
    {
        if (turnRadiusIndicator != null)
            Destroy(turnRadiusIndicator);
    }

    public void OnMouseDragStart(GameObject sender, Vector3 start)
    {
        _movePrefabInstance = GameObject.Instantiate(this.movePrefabSatellite);
        _movePrefabInstance.GetComponent<PolarCoordinateTransform>().Angle = _polarTransform.Angle;
        _movePrefabInstance.GetComponent<PolarCoordinateTransform>().Radius = _polarTransform.Radius;

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

        if (verticalLine != null)
        {
            verticalLine.enabled = true;
            verticalLine.positionCount = 2;
            verticalLine.SetPositions(new Vector3[2]
            {
                Utils.ComputeCirclePosition(_polarTransform.Angle, _polarTransform.Radius),
                Utils.ComputeCirclePosition(_polarTransform.Angle, _polarTransform.Radius)
            });
        }
    }

    public void OnMouseDrag(GameObject sender, Vector3 start, Vector3 end)
    {
        Doozy.Engine.GameEventMessage.SendEvent("HIDE_BUILD_PANELS");
        var endGameWorld = Camera.main.ScreenToWorldPoint(end);

        Vector3 currentCoords = Utils.ComputeCirclePosition(_dragAngleEnd, _dragRadiusEnd);
        var distanceFromEnd = Vector2.Distance(currentCoords, endGameWorld);

        Vector3 delta = endGameWorld - currentCoords;
        float vertAngle = Vector2.SignedAngle(Utils.ComputeCirclePosition(_dragAngleEnd, 1), delta);

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

        float angleDelta = Vector2.SignedAngle(
                                Camera.main.ScreenToWorldPoint(start),
                                Camera.main.ScreenToWorldPoint(end));

        _dragAngleEnd = AngleToNearestValid(_polarTransform.Angle + angleDelta, _dragRadiusEnd);

        turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = _polarTransform.Angle;
        turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = _dragAngleEnd;
        turnRadiusIndicator.GetComponent<CircleBehavior>().radius = _dragRadiusEnd;

        _movePrefabInstance.GetComponent<PolarCoordinateTransform>().Radius = _dragRadiusEnd;
        _movePrefabInstance.GetComponent<PolarCoordinateTransform>().Angle = _dragAngleEnd;

        verticalLine.positionCount = 2;
        verticalLine.SetPositions(new Vector3[2]
        {
            Utils.ComputeCirclePosition(_polarTransform.Angle, _polarTransform.Radius),
            Utils.ComputeCirclePosition(_polarTransform.Angle, _dragRadiusEnd)
        });
    }

    public float AngleToNearestValid(float angle, float radius)
    {
        float degPerGameUnit = 360.0f / (2.0f * radius * Mathf.PI);
        float degPerSatellite = 1.5f * degPerGameUnit;

        // Get an integer multiplier
        return Mathf.Round(angle / degPerSatellite) * degPerSatellite;
    }

    public float NextAngleToNearestValid(float angle, float count)
    {
        float degPerGameUnit = 360.0f / (2.0f * _polarTransform.Radius * Mathf.PI);
        float degPerSatellite = 1.5f * degPerGameUnit;

        // Get an integer multiplier
        return Mathf.Round((angle + degPerSatellite * count) / degPerSatellite) * (degPerSatellite);
    }

    public void OnMouseDragEnd(GameObject sender, Vector3 start, Vector3 end)
    {
        foreach (var autoAimBehavior in this.GetComponentsInChildren<AutoAimBehavior>())
            autoAimBehavior.HideRangeCircle();

        if (_movePrefabInstance != null)
        {
            Destroy(_movePrefabInstance);
            _movePrefabInstance = null;
        }

        if (CheckSatelliteAtPosition(_dragRadiusEnd, _dragAngleEnd) != null)
        {
            turnRadiusIndicator.SetActive(false);
            verticalLine.enabled = false;
            return;
        }

        MoveSatellite(_dragRadiusEnd, _dragAngleEnd);
    }

    public SatelliteBehavior CheckSatelliteAtPosition(float radius, float angle)
    {
        var vectEndPosition = Utils.ComputeCirclePosition(angle, radius);
        var satelliteLayer = LayerMask.NameToLayer("satellites");
        var itemsInRadius = Physics2D.OverlapCircleAll(vectEndPosition, 0.5f, (1 << satelliteLayer));
        if (itemsInRadius != null && itemsInRadius.Length > 0)
        {
            foreach (var item in itemsInRadius)
            {
                if (item.gameObject == this.gameObject)
                    continue;

                if (item.GetComponent<SatelliteBehavior>() == null)
                    continue;

                return item.GetComponent<SatelliteBehavior>();
            }
        }

        return null;
    }

    public void MoveSatellite(float radius, float angle, System.Action OnMoveCompleted = null)
    {
        OnMoveStarted?.Invoke(this);

        // Disable collisions when moving
        if (_boxCollider2D != null)
            _boxCollider2D.enabled = false;

        turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = 0;
        turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = 0;

        float endAngle = AngleToNearestValid(angle, radius);

        // add the final point
        if (this.endTurnIndicator != null)
        {
            if (_endTurnIndicatorInstance == null)
                _endTurnIndicatorInstance = Instantiate(this.endTurnIndicator);

            _endTurnIndicatorInstance.transform.position = Utils.ComputeCirclePosition(endAngle, radius);
        }

        float distance = (radius * 2f * Mathf.PI) * (Mathf.Abs(_polarTransform.Angle - endAngle) / 360f);
        float rotationTime = distance / rotationSpeed;

        float rotationDelta = _polarTransform.Angle - endAngle;
        System.Action doRotation = () =>
        {

            DOTween.To(() => this._polarTransform.Angle,
                    x => _polarTransform.Angle = x,
                    endAngle,
                    rotationTime)
                .OnUpdate(() =>
                    {
                        turnRadiusIndicator.GetComponent<CircleBehavior>().MinAngle = _polarTransform.Angle;
                    })
                .OnStart(() =>
                {
                    body.transform.localRotation = Quaternion.Euler(0, 0, rotationDelta > 0 ? 180f : 0f);
                    turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = endAngle;
                })
                .OnComplete(() =>
                {
                    if (CheckSatelliteAtPosition(_polarTransform.Radius, _polarTransform.Angle))
                    {
                        MoveSatellite(_polarTransform.Radius + 1, _polarTransform.Angle, OnMoveCompleted);
                        return;
                    }

                    OnMoved();
                    OnMoveEnded?.Invoke(this);
                    OnMoveCompleted?.Invoke();
                });
        };

        if (_polarTransform.Radius != radius)
        {
            float orbitdelta = _polarTransform.Radius - radius;
            if (orbitdelta > 0)
                body.transform.localRotation = Quaternion.Euler(0, 0, -90f);
            else
                body.transform.localRotation = Quaternion.Euler(0, 0, -90f);

            float increaseOrbitTime = Mathf.Abs(orbitdelta) / rotationSpeed;
            DOTween.To(() => this._polarTransform.Radius,
                            x => _polarTransform.Radius = x,
                            radius,
                            increaseOrbitTime)
                        .OnComplete(() =>
                        {
                            verticalLine.enabled = false;
                            body.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            doRotation();
                        })
                        .OnUpdate(() =>
                        {
                            verticalLine.SetPositions(new Vector3[2]
                            {
                                    Utils.ComputeCirclePosition(_polarTransform.Angle, _polarTransform.Radius),
                                    Utils.ComputeCirclePosition(_polarTransform.Angle, radius)
                            });
                        });
        }
        else
        {
            doRotation();
        }

        // Activate the exhaust
        exhaust.SetActive(true);
    }

    public void ConnectToSatellite(SatelliteBehavior satellite, LineRenderer line)
    {
        Vector3 delta = satellite.transform.position - this.transform.position;

        line.enabled = true;
        line.SetPosition(0, this.transform.position);
        line.SetPosition(1, this.transform.position);
        DOTween.To(() => line.GetPosition(1),
                    x => line.SetPosition(1, x),
                    this.transform.position + delta,
                    1f);
    }

    void OnMoved()
    {
        if (_boxCollider2D != null)
            _boxCollider2D.enabled = true;

        // Recalculate all graphs
        AstarPath.active.Scan();

        if (_endTurnIndicatorInstance != null)
        {
            Destroy(_endTurnIndicatorInstance);
            _endTurnIndicatorInstance = null;
        }

        verticalLine.enabled = false;
        if (turnRadiusIndicator != null)
            turnRadiusIndicator.SetActive(false);

        exhaust.SetActive(false);
        OnSatelliteMoved?.Invoke();

        CheckSatelliteAtPosition(_polarTransform.Radius, NextAngleToNearestValid(_polarTransform.Angle, 1));
    }

    public bool isRelocating = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var satelliteLayer = LayerMask.NameToLayer("satellites");
        if (collision.gameObject.layer == satelliteLayer)
        {
            var satellite = collision.gameObject.GetComponent<SatelliteBehavior>();
            if (satellite == null || satellite.isRelocating)
                return;

            isRelocating = true;
            MoveSatellite(_polarTransform.Radius + 1, _polarTransform.Angle, () =>
            {
                isRelocating = false;
            });
        }
    }
}
