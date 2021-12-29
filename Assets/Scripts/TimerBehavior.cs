using Doozy.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimerBehavior : MonoBehaviour
{
    public bool StartOnSpawn = true;
    public float updateFrequency = 0.2f;
    public float seconds = 10;
    float _seconds = 0;
    public TextMeshProUGUI label;
    public string LabelFormat = @"mm\:ss\.ff";
    public List<string> MessagesOnTimer = new List<string>();
    public UnityEvent OnTimer;

    Coroutine _routine = null;

    private void Start()
    {
        if(StartOnSpawn)
            Restart();
    }

    public IEnumerator co_updateTimer()
    {
        OnTimerChanged(_seconds);
        while (_seconds > 0)
        {
            yield return new WaitForSeconds(updateFrequency);
            _seconds -= updateFrequency;
            OnTimerChanged(_seconds);
            if (_seconds <= 0)
                OnTimeReached();
        }
    }

    void OnTimeReached()
    {
        foreach (var message in MessagesOnTimer)
            GameEventMessage.SendEvent(message, this.gameObject);
        OnTimer?.Invoke();
    }

    private void OnTimerChanged(float secondsLeft)
    {
        TimeSpan time = TimeSpan.FromSeconds(secondsLeft);
        if(label != null)
            label.text = time.ToString(LabelFormat);
    }

    public void Restart()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _seconds = seconds;
        _routine = StartCoroutine(co_updateTimer());
    }

    public void Stop()
    {
        if (_routine != null)
            StopCoroutine(_routine);
        _seconds = seconds;
    }
}
