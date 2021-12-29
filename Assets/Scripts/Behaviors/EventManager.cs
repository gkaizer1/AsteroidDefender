using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void CameraOrthoSizeChangedHandler(float cameraOrthoSize);
    public static event CameraOrthoSizeChangedHandler OnCameraOrthograficSizeChange;
    public static event System.Action OnWorldSizeChanged;
    public static event System.Action<GameObject> OnObjectDestroyed;

    public static void InvokeOnCameraOrthograficSizeChanged(float orthoSize)
    {
        OnCameraOrthograficSizeChange?.Invoke(orthoSize);
    }
    public static void InvokeOnWorldSizeChanged()
    {
        OnWorldSizeChanged?.Invoke();
    }
    public static void InvokeOnObjectDestroyed(GameObject gameObject)
    {
        OnObjectDestroyed?.Invoke(gameObject);
    }
}
