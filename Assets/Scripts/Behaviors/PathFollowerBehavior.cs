using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFollowerBehavior : MonoBehaviour
{
    public List<Vector3> Path = new List<Vector3>();

    [DebugProperty]
    public float Acceleration = 0.0f;
    [DebugProperty]
    public float Moment = 1000.0f;

    public float MaxSpeed = 6000.0f;
    public float MaxAcceleration = 1000.0f;

    public LineRenderer LineRenderer;

    public GameObject WayPointObject;

    public Rigidbody2D rigidBody;

    [Tooltip("World distance at which the waypoint is considered reached")]
    public float ToleranceDistance = 0.1f;

    List<GameObject> wayPoints = new List<GameObject>();

    public float MaxRotationDegreesPerSecond = 120.0f;

    void Start()
    {
        EventManager.OnObjectDestroyed += OnObjectDestroyed;
    }

    private void Update()
    {
        if (wayPoints.Count > 0)
        {
            LineRenderer.transform.position = transform.position;
            LineRenderer.SetPosition(0, transform.position);
        }
    }

    private void OnObjectDestroyed(GameObject obj)
    {
        int index = wayPoints.IndexOf(obj);
        if (index >= 0)
        {
            wayPoints.RemoveAt(index);
            Path.RemoveAt(index);
            UpdatePath();
        }
    }

    private void OnDestroy()
    {
        foreach (var wayPoints in wayPoints)
        {
            Destroy(wayPoints);
        }
        wayPoints.Clear();

        EventManager.OnObjectDestroyed -= OnObjectDestroyed;
    }

    public void AddWayPoint(Vector3 point)
    {
        var waypointLocation = new Vector3(point.x, point.y, 1.0f);
        if (WayPointObject != null)
        {
            GameObject waypoint = Instantiate(WayPointObject);
            waypoint.transform.position = waypointLocation;
            waypoint.name = $"waypoint_{name}_{Path.Count}";
            if(waypoint.GetComponent<WaypointBehavior>() != null)
                waypoint.GetComponent<WaypointBehavior>().SetParent(this.gameObject);
            wayPoints.Add(waypoint);
        }

        Path.Add(waypointLocation);
        UpdatePath();
    }

    private void UpdatePath()
    {
        LineRenderer.positionCount = Path.Count + 1;
        LineRenderer.SetPosition(0, transform.position);
        LineRenderer.startWidth = Utils.ScreenToWorldWidthPixel * 2.0f;
        LineRenderer.endWidth = Utils.ScreenToWorldWidthPixel * 2.0f;

        int position = 1;
        Path.ForEach(x =>
        {
            LineRenderer.SetPosition(position, x);
            position++;
        });
    }

    void RemoveWaypointAtIndex(int index)
    {
        // Pop the first point
        if (wayPoints.Count > index)
        {
            Destroy(wayPoints[index].gameObject);
            wayPoints.RemoveAt(index);
        }

        Path.RemoveAt(index);
        UpdatePath();

    }

    void FixedUpdate()
    {
        if (Path.Count <= 0)
            return;

        Vector2 vectCurrentVelocity = rigidBody.velocity;
        Vector3 deltaVector = Path[0] - this.transform.position;

        float distance = deltaVector.magnitude;
        if (distance < ToleranceDistance)
        {
            RemoveWaypointAtIndex(0);
            if (Path.Count <= 0)
                return;
        }

        Quaternion rotation = Quaternion.LookRotation(
               Vector3.forward, // Keep z+ pointing straight into the screen.
               new Vector3(deltaVector.x, deltaVector.y, 0.0f)      // Point y+ toward the target.
             );

        float deltaAngle = Mathf.Atan2(deltaVector.y, deltaVector.x);
        if (Mathf.Abs(deltaAngle) > 0.01f)
        {
            // Apply a compensating rotation that twists x+ to y+ before the rotation above.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, MaxRotationDegreesPerSecond * Time.deltaTime);
            rigidBody.velocity = rigidBody.velocity.magnitude * transform.up;

           // rigidBody.velocity = transform.rotation * rigidBody.velocity;
        }
        else
        {
            // If we are NOT turning then accelerate in the desired direction
            if (vectCurrentVelocity.magnitude < MaxSpeed)
            {
                Acceleration = Mathf.Clamp(Acceleration + Moment * Time.deltaTime, 0.0f, MaxAcceleration);
                Vector2 up = rigidBody.transform.up.normalized;
                rigidBody.velocity += up * Acceleration * Time.deltaTime;
            }
            else
            {
                Acceleration = 0.0f;
                rigidBody.velocity = rigidBody.velocity.normalized * MaxSpeed;
            }

        }
    }
}
