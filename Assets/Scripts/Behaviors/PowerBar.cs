using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PowerBar : MonoBehaviour
{
    public GameObject outOfPowerIndicator;
    public Transform powerBar;

    public float minPowerRequired = 0.0f;
    public void OnPowerChanged(float currentPower, float maxPower)
    {
        powerBar.localScale = new Vector3(
            maxPower != 0 ? Mathf.Clamp01(currentPower / maxPower) : 0f,
            powerBar.localScale.y,
            powerBar.localScale.z);

        if(currentPower <= minPowerRequired)
        {
            if(!outOfPowerIndicator.activeSelf)
                outOfPowerIndicator.SetActive(true);
        }
        else
        {
            if (outOfPowerIndicator.activeSelf)
                outOfPowerIndicator.SetActive(false);
        }
    }
}
