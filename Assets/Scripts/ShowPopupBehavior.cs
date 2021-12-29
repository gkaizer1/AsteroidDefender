using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShowPopupBehavior : MonoBehaviour
{
    public bool ShowOnWake = true;
    public string PopupToShow;
    UIPopup _popup = null;
    public UnityEvent OnPopupClosed;

    public void Start()
    {
        if (ShowOnWake)
            ShowPopup();
    }

    public void ShowPopup()
    {
        _popup = UIPopup.GetPopup(PopupToShow);
        UIPopupManager.AddToQueue(_popup);

        StartCoroutine(WaitForPopupToClose());
    }

    IEnumerator WaitForPopupToClose()
    {
        while(_popup != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        OnPopupClosed?.Invoke();
    }
}
