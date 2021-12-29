using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AstroidManager : MonoBehaviour
{
    public static List<GameObject> Astroids = new List<GameObject>();
    public static AstroidManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(UpdateAstroidsInsideEearth());
    }

    IEnumerator UpdateAstroidsInsideEearth()
    {
        while (this.gameObject != null)
        {
            foreach(var astroid in Astroids)
            {
                var astroidBehavior = astroid.GetComponent<Astroid>();
                if (astroidBehavior == null)
                    continue;

                Vector2 delta = astroidBehavior._targetPoint - astroidBehavior.transform.position;
                astroidBehavior.IsInsideEarth = (astroidBehavior.transform.position.magnitude < astroidBehavior._earthCollisionRadius);

                astroidBehavior.IsOnCourse = (astroidBehavior.rigidBody.velocity).magnitude > 0.01f && (delta.normalized - astroidBehavior.rigidBody.velocity.normalized).magnitude < 0.1f;

            }

            yield return new WaitForSeconds(0.2f);
        }
    }

}
