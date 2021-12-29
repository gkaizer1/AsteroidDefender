using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SuperNukeTimerPanelBehavior : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    void Awake()
    {
        CommandCenterBehavior.OnTimeToSuperNukeChanged += OnSuperNukeCounterChanged;
    }

    private void OnSuperNukeCounterChanged(float secondsToNuke)
    {
        TimeSpan time = TimeSpan.FromSeconds(secondsToNuke);
        textObject.text = time.ToString(@"mm\:ss");
    }
}
