using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEventBehavior : MonoBehaviour
{
    public string Event = "START_LEVEL";

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
            Destroy(this.gameObject);
    }
}