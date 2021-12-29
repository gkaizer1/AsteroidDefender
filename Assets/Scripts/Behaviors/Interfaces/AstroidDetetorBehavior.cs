using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AstroidDetetorBehavior : MonoBehaviour
{

    public float AutoDetectRange = 15.0f;

    void Update()
    {
        var astroids = GameObject.FindGameObjectsWithTag("astroid").ToList();
        astroids.ForEach(gameObject =>
        {
            var astroid = gameObject.GetComponent<Astroid>();
            float distance = (this.transform.position - astroid.transform.position).magnitude;


            if (distance < AutoDetectRange)
            {
                astroid.GetComponent<DetectableBehavior>().SetDetectedPrecentage(1.0f);
            }
            else
            {
                float precentage = astroid.GetComponent<DetectableBehavior>().DetectedPrecentage + Time.deltaTime * 1.0f /Mathf.Abs(distance - AutoDetectRange);
                precentage = Mathf.Clamp01(precentage);
                astroid.GetComponent<DetectableBehavior>().SetDetectedPrecentage(precentage);
            }            
        });
    }
}
