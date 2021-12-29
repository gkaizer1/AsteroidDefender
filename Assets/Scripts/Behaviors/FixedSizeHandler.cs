using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FixedSizeHandler : MonoBehaviour
{
    public float FixedSizeWith = 64;
    public float FixedSizeHeight = 64;

    float _previousWidth;
    float _previousHeight;

    float previousOrthoSize = 0.0f;

    private void Start()
    {
        EventManager.OnCameraOrthograficSizeChange += OnCameraOrthoSizeChanged;
        OnCameraOrthoSizeChanged(Camera.main.orthographicSize);
    }

    private void OnDestroy()
    {
        EventManager.OnCameraOrthograficSizeChange -= OnCameraOrthoSizeChanged;
    }

    private void OnCameraOrthoSizeChanged(float cameraOrthoSize)
    {
    }

    void Update()
    {
        if (previousOrthoSize == Camera.main.orthographicSize && _previousWidth == FixedSizeWith && _previousHeight == FixedSizeHeight)
            return;

        previousOrthoSize = Camera.main.orthographicSize;
        try
        {
            var renderer = GetComponent<SpriteRenderer>();
            var previousRotation = transform.rotation;
            transform.localScale = new Vector3(1, 1, 1);
            transform.rotation = new Quaternion();
            var pixelHeight = renderer.bounds.size.y / Utils.ScreenToWorldHeightPixel;
            var factor = (float)FixedSizeHeight / (float)pixelHeight;
            if (Math.Abs(factor - 1.0f) > 0.01f)
            {
                transform.localScale = factor * transform.localScale;
            }
            transform.rotation = previousRotation;
            _previousHeight = FixedSizeHeight;
            _previousWidth = FixedSizeWith;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
