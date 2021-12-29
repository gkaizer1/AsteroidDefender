using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyBehavior : MonoBehaviour
{
    public float destroyInSeconds = 5.0f;
    public bool startOnEnabled = false;
    public void OnEnable()
    {
        if (startOnEnabled)
            DestroySelf();
    }

    public void DestroySelf()
    {
        if(this.gameObject.activeSelf)
            StartCoroutine(co_Destroy());
    }

    public IEnumerator co_Destroy()
    {
        yield return new WaitForSeconds(destroyInSeconds);
        Destroy(this.gameObject);
    }
}
