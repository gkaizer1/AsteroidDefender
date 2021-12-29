using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AutoFireEvent : UnityEvent
{
}

public class AutoFireBehavior : MonoBehaviour
{
    public AutoFireEvent OnAutoFire;

    public float fireRate = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoFireRoutine());
    }

    IEnumerator AutoFireRoutine()
    {
        while (this.gameObject != null)
        {
            OnAutoFire?.Invoke();
            yield return new WaitForSeconds(1.0f / fireRate);
        }
    }
}
