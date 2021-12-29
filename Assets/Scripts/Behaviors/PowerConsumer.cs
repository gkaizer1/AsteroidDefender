using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PowerChangedEvent : UnityEvent<float, float>
{
}

public class PowerConsumer : MonoBehaviour
{
    public float MaxPower = 10.0f;
    public float _CurrentPower = 0.0f;
    public float MaxPowerDraw = 100.0f;

    public float CurrentPower
    {
        get
        {
            return _CurrentPower;
        }
        set
        {
            _CurrentPower = value;
            OnPowerChanged?.Invoke(_CurrentPower, MaxPower);
        }
    }

    public PowerChangedEvent OnPowerChanged;

    private void Start()
    {
        OnPowerChanged?.Invoke(_CurrentPower, MaxPower);
    }

    /*
     * Given an amount of power return amount used
     */
    public float ProvidePower(float power)
    {
        float newPower = Mathf.Clamp(_CurrentPower + Mathf.Clamp(power, 0f, MaxPowerDraw), 0.0f, MaxPower);
        float powerUsed = newPower - _CurrentPower;
        if (_CurrentPower != newPower)
        {
            _CurrentPower = newPower;
            OnPowerChanged?.Invoke(_CurrentPower, MaxPower);
        }

        return powerUsed;
    }
}