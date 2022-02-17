using Doozy.Engine;
using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupListener : MonoBehaviour
{
    // Start is called before the first frame update
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

        if (message.EventName.StartsWith("SHOW_POPUP."))
        {
            string popupName = message.EventName.Replace("SHOW_POPUP.", "");
            var popup = UIPopup.GetPopup(popupName);
            UIPopupManager.AddToQueue(popup);
        }
    }
}
