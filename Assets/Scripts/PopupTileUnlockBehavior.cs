using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTileUnlockBehavior : MonoBehaviour
{
    public event System.Action OnDestroyEvent;

    public void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
}
