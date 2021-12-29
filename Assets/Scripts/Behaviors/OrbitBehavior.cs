using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Doozy.Engine;

public class OrbitBehavior : MonoBehaviour
{
    [Header("Children")]
    public GameObject circle;
    public GameObject turnRadiusIndicator;
    public GameObject rotator;

    [Header("Prefabs")]
    public GameObject emptySatellitePrefab;
    public DragableBehavior rotatorHandle;

    public float OrbitDistance = 10.0f;
    public float orbitSpeed = 10.0f;

    public List<GameObject> objectsToHide = new List<GameObject>();
    public List<GameObject> turnIndicators = new List<GameObject>();

    public List<GameObject> satellites = new List<GameObject>();

    float _rotationStart = 0.0f;
    public float rotationSpeed = 5f;
    Tween _rotationTween = null;

    public bool show = false;
    public int orbitIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        circle.transform.localScale = Vector3.one * OrbitDistance;
        turnRadiusIndicator.GetComponent<CircleBehavior>().radius = OrbitDistance;

        rotator.GetComponent<RotatorBehavior>().DegreePerSecond = orbitSpeed;

        float circumference = 2 * Mathf.PI * OrbitDistance;
        float satelliteCount = Mathf.Clamp(circumference / 10.0f, 1, float.MaxValue);
        float incrementAngle = 360.0f / satelliteCount;

        int i = 0;
        for (float angle = 0; angle < 359.9f; angle += (int)incrementAngle)
        {
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            if ((satellites.Count - 1) < i)
            {
                //var satellite = Instantiate(buildSatellitePrefab, rotator.transform);
                //satellite.transform.position = new Vector3(x * OrbitDistance, y * OrbitDistance, 1f);
                //objectsToHide.Add(satellite);
            }
            else
            {
                //var satellite = Instantiate(satellites[i], rotator.transform);
                //var satelliteBehavior = satellite.GetComponent<PolarCoordinateTransform>();
                //satelliteBehavior.OrbitDistance = OrbitDistance;
                //satelliteBehavior.OrbitAngle = angle;
            }

            i++;
        }

        objectsToHide.Add(circle.gameObject);
        turnRadiusIndicator.SetActive(false);
        Show(show);

        Message.AddListener<GameEventMessage>(OnMessage);
    }

    public GameObject UnlockSatellite(int index)
    {
        float circumference = 2 * Mathf.PI * OrbitDistance;
        float satelliteCount = Mathf.Clamp(circumference / 10.0f, 1, float.MaxValue);
        float incrementAngle = 360.0f / satelliteCount;

        float unlockAngle = incrementAngle * index;
        float x = Mathf.Cos(unlockAngle * Mathf.Deg2Rad);
        float y = Mathf.Sin(unlockAngle * Mathf.Deg2Rad);

        var satellite = Instantiate(GlobalGameSettings.Instance.EmptySatellitePrefab, rotator.transform);
        satellite.transform.position = new Vector3(x * OrbitDistance, y * OrbitDistance, 1f);
        return satellite;
    }

    private void OnMouseDragStart(GameObject sender, Vector3 start)
    {
        foreach (var autoAimBehavior in this.GetComponentsInChildren<AutoAimBehavior>())
            autoAimBehavior.ShowRangeCircle();

        foreach (var indicator in turnIndicators)
        {
            if (indicator == sender)
                continue;
            indicator.SetActive(false);
        }

        turnRadiusIndicator.SetActive(true);
        _rotationStart = rotator.transform.rotation.eulerAngles.z;

        float angle = Vector2.SignedAngle(
            Vector2.right,
            Camera.main.ScreenToWorldPoint(start));
        turnRadiusIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnMouseDragEnd(GameObject sender, Vector3 start, Vector3 end)
    {
        foreach (var autoAimBehavior in this.GetComponentsInChildren<AutoAimBehavior>())
            autoAimBehavior.HideRangeCircle();

        _rotationTween = rotator.transform.DORotate(new Vector3(0f, 0f, _rotationStart), 0.25f)
            .OnComplete(() =>
            {
                float angle = Vector2.SignedAngle(
                    Camera.main.ScreenToWorldPoint(start),
                    Camera.main.ScreenToWorldPoint(end));

                float initialAngle = rotator.transform.rotation.eulerAngles.z;
                float finalRotationAngle = initialAngle + angle;

                float startAngle = Vector2.SignedAngle(
                    Vector2.right,
                    Camera.main.ScreenToWorldPoint(start));
                turnRadiusIndicator.transform.rotation = Quaternion.Euler(0, 0, startAngle);
                turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = 0;

                float distance = (OrbitDistance * 2f * Mathf.PI) * (Mathf.Abs(angle) / 360f);
                float rotationTime = distance / rotationSpeed;
                _rotationTween = rotator.transform.DORotate(new Vector3(0f, 0f, finalRotationAngle), rotationTime).OnUpdate(() =>
                {

                    // set the turn start as the StartAnge + Travelled
                    float currentAngle = rotator.transform.rotation.eulerAngles.z;
                    float angleTravelled = (currentAngle - initialAngle);
                    turnRadiusIndicator.transform.rotation = Quaternion.Euler(0, 0, startAngle + angleTravelled);

                    // set the left over radius as FinalAngle - CurrentAngle
                    float deltaAngle = Mathf.DeltaAngle(rotator.transform.rotation.eulerAngles.z, finalRotationAngle);
                    turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = deltaAngle;
                })
                .OnComplete(() =>
                {
                    turnRadiusIndicator.SetActive(false);
                    _rotationTween = null;

                    foreach (var indicator in turnIndicators)
                        indicator.SetActive(true);
                });
            });
    }

    private void OnMouseDrag(GameObject sender, Vector3 start, Vector3 end)
    {
        if (_rotationTween != null)
        {
            _rotationTween.Kill();
            _rotationTween = null;
        }

        float angle = Vector2.SignedAngle(
            Camera.main.ScreenToWorldPoint(start),
            Camera.main.ScreenToWorldPoint(end));
        turnRadiusIndicator.GetComponent<CircleBehavior>().MaxAngle = angle;
        rotator.transform.rotation = Quaternion.Euler(0f, 0f, _rotationStart + angle);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<GameEventMessage>(OnMessage);
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        if (message.EventName == "SHOW_ORBITS")
        {
            Show(true);
        }
        if (message.EventName == "HIDE_ORBITS")
        {
            Show(false);
        }
    }

    public void Show(bool show)
    {
        foreach (var x in objectsToHide)
        {
            x.SetActive(show);
        }
    }
}
