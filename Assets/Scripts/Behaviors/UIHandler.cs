using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void OnSlowdownTimeHanlder()
    {
        Time.timeScale -= 0.1f;
    }

    // Update is called once per frame
    void OnSpeedupTime()
    {
        Time.timeScale += 0.1f;
    }
}
