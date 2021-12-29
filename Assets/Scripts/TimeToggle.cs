using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToggle : MonoBehaviour
{
    public float toggleTime = 0.1f;
    public SpriteRenderer objectToToggle = null;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(co_toggle());
    }

    IEnumerator co_toggle()
    {
        var time = new WaitForSeconds(toggleTime);
        while (true)
        {
            objectToToggle.enabled = !objectToToggle.enabled;
            yield return time;
        }
    }
}
