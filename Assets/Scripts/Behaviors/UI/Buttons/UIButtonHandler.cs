using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    public void OnSlowdownTimeClicked()
    {
        Time.timeScale -= 0.1f;
    }
    public void OnSpeedUpTimeClicked()
    {
        Time.timeScale += 0.1f;
    }
    
    public void OnPause()
    {
        Time.timeScale = 0.0f;
    }

    public void OnPlay()
    {
        Time.timeScale = 1.0f;
    }

    public void OnPlayFast()
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale + 1.0f, 2.0f, 5.0f);
    }
}
