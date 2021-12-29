using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[CreateAssetMenu(fileName = "DebugOverlay", menuName = "Debug/Overlay")]
public class DebugOverlay : ScriptableObject
{
    public void Create()
    {
        // Canvas
        GameObject gameObject = new GameObject();
        gameObject.name = "DEBUG_CANVAS";
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.AddComponent<Canvas>();

        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent(typeof(DebugCanvas));
        canvas.sortingLayerName = "UI";


        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();
    }
}
