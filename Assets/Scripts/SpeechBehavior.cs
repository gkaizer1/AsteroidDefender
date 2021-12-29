using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpeechBehavior : MonoBehaviour
{
    public string title;
    [TextArea]
    public string text;

    public UnityEvent OnSpeechEnded;

    PopupSpeechBehavior _popup;

    public bool showOnStartup = true;

    bool _shown = false;

    private void Start()
    {
        if (showOnStartup)
        {
            ShowSpeech();
        }
    }

    public void ShowSpeech()
    {
        if (_shown)
            return;

        _shown = true;
        _popup = PopupSpeechBehavior.CreateSpeechDialog(title, text);
        _popup.OnDestroyed += OnSpeechDestroyed;
    }

    private void OnSpeechDestroyed()
    {
        _shown = false;
        _popup.OnDestroyed -= OnSpeechDestroyed;
        OnSpeechEnded?.Invoke();
    }
}
