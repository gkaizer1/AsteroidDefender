using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VelocityPanelBehavior : MonoBehaviour, ISelectionPanel
{
    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public Rigidbody2D rigidBody;

    [Header("UI")]
    public TextMeshProUGUI velocityValueLabel;

    public void SetTarget(GameObject gameObject)
    {
        target = gameObject;
        rigidBody = target.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(rigidBody != null)
            velocityValueLabel.text = Utils.VelocityToString(rigidBody.velocity.magnitude);
    }
}