using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerIndicatorBehavior : MonoBehaviour
{
    public List<GameObject> powerLights = new List<GameObject>();

    public void OnPowerChanged(float current, float max)
    {
        float precentage = current / max;
        if (precentage > 0.9)
        {
            powerLights[0].SetActive(true);
            powerLights[1].SetActive(true);
            powerLights[2].SetActive(true);
        }
        else if (precentage > 0.5)
        {
            powerLights[0].SetActive(true);
            powerLights[1].SetActive(true);
            powerLights[2].SetActive(false);
        }
        else if (precentage > 0.1)
        {
            powerLights[0].SetActive(true);
            powerLights[1].SetActive(false);
            powerLights[2].SetActive(false);
        }
        else
        {
            powerLights[0].SetActive(false);
            powerLights[1].SetActive(false);
            powerLights[2].SetActive(false);
        }
    }
}
