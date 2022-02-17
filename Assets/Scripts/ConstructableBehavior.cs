using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConstructableBehavior : MonoBehaviour
{
    public UnityEvent<GameObject> OnConstructionCompleted;

    public GameObject ObjectToConstructPrefab;

    public float constructionTime = 5.0f;
    private float _elapsedTime = 0.0f;

    public bool IsBuilding = false;
    public bool RequiresShuttle = false;

    // Update is called once per frame
    void Update()
    {
        if (!IsBuilding)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= constructionTime)
        {
            GameObject constructedObject = GameObject.Instantiate(ObjectToConstructPrefab);
            constructedObject.transform.position = this.transform.position;

            var polarCoodinates = this.GetComponent<PolarCoordinateTransform>();
            var childPolarCoodinates = constructedObject.GetComponent<PolarCoordinateTransform>();
            if (polarCoodinates != null && childPolarCoodinates != null)
            {
                childPolarCoodinates.Angle = polarCoodinates.Angle;
                childPolarCoodinates.Radius = polarCoodinates.Radius;
            }
            OnConstructionCompleted?.Invoke(constructedObject);

            if (gameObject != null)
                Destroy(gameObject);
        }
    }
}
