using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{

    public static float GetScreenToWorldHeight
    {
        get
        {
            Vector2 topRightCorner = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner); var height = edgeVector.y * 2;
            return height;
        }
    }
    public static float GetScreenToWorldWidth
    {
        get
        {
            Vector2 topRightCorner = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner); var width = edgeVector.x * 2;
            return width;
        }
    }
    void Update()
    {
        this.transform.localScale = new Vector2(Utils.AspecRatio * Camera.main.orthographicSize * 1.5f, Camera.main.orthographicSize * 2.0f);
    }
}
