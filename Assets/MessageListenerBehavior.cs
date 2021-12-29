using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MessageEvent
{
    public string Event;
    public UnityEvent Callbacks;
}

public class MessageListenerBehavior : MonoBehaviour
{
    public List<MessageEvent> Messages = new List<MessageEvent>();

    // Start is called before the first frame update
    void Awake()
    {
        GameEventMessage.AddListener<GameEventMessage>(OnMessage);
    }

    public void OnDestroy()
    {
        GameEventMessage.RemoveListener<GameEventMessage>(OnMessage);
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        string eventName = message.EventName;
        foreach(var handler in Messages)
        {
            if (handler.Event == eventName)
                handler.Callbacks?.Invoke();
        }
    }
}
