using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperNukeBehavior : MonoBehaviour
{
    float _initialDistanceToTarget = 0.0f;
    Transform _transform;
    Animator _animator;

    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _transform = this.transform;
        this._initialDistanceToTarget = (_transform.position - this.GetComponent<AutoAimBehavior>().currentTarget.transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<AutoAimBehavior>().currentTarget != null)
        {
            float currentDistance = (_transform.position - this.GetComponent<AutoAimBehavior>().currentTarget.transform.position).magnitude;
            _animator.SetFloat("animSpeed", (1.0f - currentDistance / _initialDistanceToTarget) * 15.0f);
        }
    }
}
