using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IndicatorBehavior : MonoBehaviour
{
    [Header("Prefabs")]
    public Transform anchor_point;
    public bool AlwaysShow = true;

    Camera _camera = null;

    public UnityEvent onIndicatorClicked;
    public float updateFrequency = 0.1f;

    public GameObject uiContainer = null;
    public float ShowDistance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (anchor_point == null)
            anchor_point = this.transform;

        var canvas = GameObject.FindGameObjectsWithTag("cavas_indicators")[0] as GameObject;
        this.transform.SetParent(canvas.transform, true);
        _camera = Camera.main;

        StartCoroutine(co_UpdateIndicator());
    }

    // Update is called once per frame
    IEnumerator co_UpdateIndicator()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateFrequency);
            if (anchor_point == null)
            {
                Destroy(this);
                yield return null;
            }
            var bounds = Utils.GetOrthographicBounds(_camera);
            bool inBounds = bounds.Contains(new Vector2(anchor_point.position.x, anchor_point.position.y));

            if(!inBounds && ShowDistance != 0f)
            {
                float distance = Vector3.Distance(anchor_point.position, _camera.transform.position) - (Mathf.Max(bounds.height, bounds.width) / 2f);
                if (distance < ShowDistance)
                    inBounds = true;
            }

            if (inBounds)
            {
                if (AlwaysShow)
                {
                    if (uiContainer != null)
                        uiContainer.SetActive(true);
                    this.transform.rotation = Quaternion.Euler(0, 0, 90f);

                    var position = Camera.main.WorldToScreenPoint(anchor_point.position);
                    this.transform.position = position;// + new Vector3(0, 32, 0);
                }
                else
                {
                    if (uiContainer != null)
                        uiContainer.SetActive(false);
                }
                continue;
            }
            else
            {
                if (uiContainer != null)
                    uiContainer.SetActive(true);
            }

            Vector3 cameraPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f);
            Vector3 direction = cameraPosition - anchor_point.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector2 intersection = Vector3.zero;
            bool intersects = false;

            // Intersect with bottom bounds
            if (!intersects)
            {
                intersects = Utils.IntersectLineSegments2D(
                                new Vector3(cameraPosition.x, cameraPosition.y),
                                new Vector3(anchor_point.position.x, anchor_point.position.y),
                                new Vector3(bounds.xMin, bounds.yMin),
                                new Vector3(bounds.xMax, bounds.yMin),
                                out intersection);
            }

            // Intersect with left bounds
            if (!intersects)
            {
                intersects = Utils.IntersectLineSegments2D(
                                new Vector3(cameraPosition.x, cameraPosition.y),
                                new Vector3(anchor_point.position.x, anchor_point.position.y),
                                new Vector3(bounds.xMin, bounds.yMin),
                                new Vector3(bounds.xMin, bounds.yMax),
                                out intersection);
            }
            // Intersect with right bounds
            if (!intersects)
            {
                intersects = Utils.IntersectLineSegments2D(
                                new Vector3(cameraPosition.x, cameraPosition.y),
                                new Vector3(anchor_point.position.x, anchor_point.position.y),
                                new Vector3(bounds.xMax, bounds.yMin),
                                new Vector3(bounds.xMax, bounds.yMax),
                                out intersection);
            }

            // Intersect with top bounds
            if (!intersects)
            {
                intersects = Utils.IntersectLineSegments2D(
                                new Vector3(cameraPosition.x, cameraPosition.y),
                                new Vector3(anchor_point.position.x, anchor_point.position.y),
                                new Vector3(bounds.xMin, bounds.yMax),
                                new Vector3(bounds.xMax, bounds.yMax),
                                out intersection);
            }

            if (!intersects)
            {
                continue;
            }

            var canvasPosition = Camera.main.WorldToScreenPoint(intersection);

            Vector3 worldCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
            Vector3 canvasDir = canvasPosition - worldCenter;
            this.transform.position = canvasPosition - (canvasDir.normalized * 16.0f);
        }
    }

    public void OnIndicatorButtonClicked()
    {
        onIndicatorClicked?.Invoke();
    }
}
