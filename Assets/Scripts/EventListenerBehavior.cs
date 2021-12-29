using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventListenerBehavior : MonoBehaviour
{
    public string Event = "START_LEVEL";
    public UnityEvent OnEvent;

    void Start()
    {
        Message.AddListener<GameEventMessage>(OnMessage);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<GameEventMessage>(OnMessage);
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        if (message.EventName == Event)
            OnEvent?.Invoke();
    }
}
