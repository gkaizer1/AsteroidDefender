using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct PolarCoodinates
{
    public float radius;
    public float angle;

    public override string ToString()
    {
        return $"Angle={angle}, Radius={radius}";
    }
}

public static class Utils
{
    public static float AngleToNearestValid(float angle, float radius)
    {
        float degPerGameUnit = 360.0f / (2.0f * radius * Mathf.PI);
        float degPerSatellite = 1.5f * degPerGameUnit;

        // Get an integer multiplier
        return Mathf.Round(angle / degPerSatellite) * degPerSatellite;
    }
    public static float NextValidAngle(float angle, float radius, float count = 1)
    {
        float degPerGameUnit = 360.0f / (2.0f * radius * Mathf.PI);
        float degPerSatellite = 1.5f * degPerGameUnit;

        // Get an integer multiplier
        return Mathf.Round((angle + degPerSatellite * count) / degPerSatellite) * (degPerSatellite);
    }

    public static Vector3 PolarToCartesian(float angle, float radius)
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        return new Vector3(x, y, 1f);
    }
    public static PolarCoodinates CartesianToPolar(Vector2 position)
    {
        float angle = (Mathf.Rad2Deg * Mathf.Atan2(position.y, position.x)) % 360.0f;
        if(angle < 0)
        {
            angle = angle + 360.0f;
        }

        return new PolarCoodinates()
        {
            radius = Mathf.Sqrt((position.x * position.x) + (position.y * position.y)),
            angle = angle
        };
    }

    public static bool IsOverUIElement()
    {
        List<GameObject> objectsToIgnore = ClickHandler.Instance?.UI_OBJECTS_TO_IGNORE ?? new List<GameObject>(); ;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var go in raycastResults)
                {
                    if (objectsToIgnore?.Contains(go.gameObject) ?? false)
                        continue;

                    if (go.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
                        return true;

                    return true;
                }
            }
        }

        return false;
    }
    public static void UnassignedReference(this GameObject go, string referenceName)
    {
        Debug.LogError("Unassigned Inspector reference: " + referenceName, go);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public static List<GameObject> GetTilesInCircle(Vector2 center, float radius)
    {
        int tilesLayer = LayerMask.NameToLayer("Tiles");
        List<int> physicsMask = new List<int>()
        {
            1 << tilesLayer
        };

        List<GameObject> tiles = new List<GameObject>();
        foreach (var mask in physicsMask)
        {
            var itemsInRadius = Physics2D.OverlapCircleAll(center, radius, mask);
            foreach (var item in itemsInRadius)
            {
                var tileBehavior = item.gameObject.GetComponent<TileBehavior>();
                if (tileBehavior != null)
                    tiles.Add(item.gameObject);
            }
        }
        return tiles;
    }

    public static List<GameObject> GetEnemiesInCircle(Vector2 center, float radius)
    {
        int no_collision_layer = LayerMask.NameToLayer("astroids_no_collisions");
        int astroid_layer = LayerMask.NameToLayer("enemies");
        List<int> physicsMask = new List<int>()
        {
            1 << astroid_layer,
            1 << no_collision_layer,
        };

        List<GameObject> enemies = new List<GameObject>();
        foreach (var mask in physicsMask)
        {
            var itemsInRadius = Physics2D.OverlapCircleAll(center, radius, mask);
            foreach(var item in itemsInRadius)
            {
                var enemyBehavior = item.gameObject.GetComponent<EnemyBehavior>();
                if (enemyBehavior != null && enemyBehavior.IsActive)
                    enemies.Add(item.gameObject);
            }
        }
        return enemies;
    }

    public static Vector2 GenerateClosestPointToEarth(Vector3 Origin)
    {
        float angle = Vector2.SignedAngle(Vector2.right, Origin);
        var earth = GameObject.FindGameObjectWithTag("earth");
        var earthCircle = earth.GetComponent<CircleCollider2D>();
        float earthRadius = earthCircle.radius * 1.1f;
        return new Vector2(earthRadius * Mathf.Cos(Mathf.Deg2Rad * angle), earthRadius * Mathf.Sin(Mathf.Deg2Rad * angle));
    }

    public static Vector2 GenerateRandomPointInEarth()
    {
        var earth = GameObject.FindGameObjectWithTag("earth");
        var earthCircle = earth.GetComponent<CircleCollider2D>();
        float randomRadius = UnityEngine.Random.Range(0, earthCircle.radius) * 0.9f;
        float randomAngle = UnityEngine.Random.Range(0, 360);
        var randomPoint = new Vector2(
            randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad), 
            randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad));

        return randomPoint;
    }

    public static bool IsParent(this GameObject obj, GameObject parent)
    {
        // If any parent is selected then we are selected as well       
        GameObject current = obj.gameObject;
        while (current != null)
        {
            if (current == parent)
                return true;

            if (current.transform.parent == null)
                break;
            
            current = current.transform.parent.gameObject;
        }
        return false;
    }
    public static float GetAudioVolumeFromCamera(float maxDistance = 50.0f)
    {
        return Mathf.Clamp01((maxDistance - Camera.main.orthographicSize) / maxDistance);
    }

    public static Vector2 rotateVector2(Vector2 v, float delta)
    {
        delta = delta * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
    public static Bounds GetMaxBounds(GameObject g)
    {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<SpriteRenderer>())
        {
            if (r.enabled == false || r is LineRenderer)
                continue;

            b.Encapsulate(r.bounds);
        }
        return b;
    }
    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
    public static bool Approximately(float a, float b, float tolerance = 1e-5f)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }

    public static float CrossProduct2D(Vector2 a, Vector2 b)
    {
        return a.x * b.y - b.x * a.y;
    }

    /// <summary>
    /// Determine whether 2 lines intersect, and give the intersection point if so.
    /// </summary>
    /// <param name="p1start">Start point of the first line</param>
    /// <param name="p1end">End point of the first line</param>
    /// <param name="p2start">Start point of the second line</param>
    /// <param name="p2end">End point of the second line</param>
    /// <param name="intersection">If there is an intersection, this will be populated with the point</param>
    /// <returns>True if the lines intersect, false otherwise.</returns>
    public static bool IntersectLineSegments2D(Vector2 p1start, Vector2 p1end, Vector2 p2start, Vector2 p2end,
        out Vector2 intersection)
    {
        // Consider:
        //   p1start = p
        //   p1end = p + r
        //   p2start = q
        //   p2end = q + s
        // We want to find the intersection point where :
        //  p + t*r == q + u*s
        // So we need to solve for t and u
        var p = p1start;
        var r = p1end - p1start;
        var q = p2start;
        var s = p2end - p2start;
        var qminusp = q - p;

        float cross_rs = CrossProduct2D(r, s);

        if (Approximately(cross_rs, 0f))
        {
            // Parallel lines
            if (Approximately(CrossProduct2D(qminusp, r), 0f))
            {
                // Co-linear lines, could overlap
                float rdotr = Vector2.Dot(r, r);
                float sdotr = Vector2.Dot(s, r);
                // this means lines are co-linear
                // they may or may not be overlapping
                float t0 = Vector2.Dot(qminusp, r / rdotr);
                float t1 = t0 + sdotr / rdotr;
                if (sdotr < 0)
                {
                    // lines were facing in different directions so t1 > t0, swap to simplify check
                    Swap(ref t0, ref t1);
                }

                if (t0 <= 1 && t1 >= 0)
                {
                    // Nice half-way point intersection
                    float t = Mathf.Lerp(Mathf.Max(0, t0), Mathf.Min(1, t1), 0.5f);
                    intersection = p + t * r;
                    return true;
                }
                else
                {
                    // Co-linear but disjoint
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else
            {
                // Just parallel in different places, cannot intersect
                intersection = Vector2.zero;
                return false;
            }
        }
        else
        {
            // Not parallel, calculate t and u
            float t = CrossProduct2D(qminusp, s) / cross_rs;
            float u = CrossProduct2D(qminusp, r) / cross_rs;
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                intersection = p + t * r;
                return true;
            }
            else
            {
                // Lines only cross outside segment range
                intersection = Vector2.zero;
                return false;
            }
        }
    }

    public static Rect GetOrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        float cameraWidth = cameraHeight * screenAspect;

        Vector2 topLeft = new Vector2(
            camera.transform.position.x - (cameraWidth / 2.0f),
            camera.transform.position.y - (cameraHeight / 2.0f));

        Rect bounds = new Rect(topLeft, new Vector2(cameraHeight * screenAspect, cameraHeight));
        return bounds;
    }
    public static string VelocityToString(float velocity)
    {
        string formattedVelocity = (velocity * 10000.0f).ToString("0.00");
        return $"{formattedVelocity} Mph";
    }
    public static float AspecRatio
    {
        get
        {
            return (float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight;
        }
    }
    public static float ScreenToWorldHeightPixel
    {
        get
        {
            var height = (Camera.main.orthographicSize * 2.0f) / Camera.main.pixelHeight;
            return height;
        }
    }
    public static float ScreenToWorldWidthPixel
    {
        get
        {
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, Camera.main.nearClipPlane));
            Vector2 edgeVector2 = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, Camera.main.nearClipPlane));
            var width = Math.Abs(edgeVector2.x - edgeVector.x) / Camera.main.pixelWidth;
            return width;
        }
    }
    public static float Distance(GameObject obj1, GameObject obj2)
    {
        return Mathf.Sqrt(Mathf.Pow(obj1.transform.position.x - obj2.transform.position.x, 2) + Mathf.Pow(obj1.transform.position.y - obj2.transform.position.y, 2));
    }

    public static float Distance(GameObject obj1, Vector3 obj2)
    {
        return Mathf.Sqrt(Mathf.Pow(obj1.transform.position.x - obj2.x, 2) + Mathf.Pow(obj1.transform.position.y - obj2.y, 2));
    }

    public static float GetAngle(GameObject obj1, GameObject obj2)
    {
        float sin = obj1.transform.position.x * obj2.transform.position.y - obj2.transform.position.x * obj1.transform.position.y;
        float cos = obj1.transform.position.x * obj2.transform.position.x + obj1.transform.position.y * obj2.transform.position.y;

        return Mathf.Atan2(sin, cos);
    }
    public static float GetAngle(Vector3 obj1, Vector3 obj2)
    {
        float sin = obj1.x * obj2.y - obj2.x * obj1.y;
        float cos = obj1.x * obj2.x + obj1.y * obj2.y;

        return Mathf.Atan2(sin, cos);
    }

}
