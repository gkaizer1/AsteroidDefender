using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicatorBehavior : MonoBehaviour
{
    public LineRenderer _lineRenderer;
    public GameObject target;
    public float lineWidth = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        if(_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        EventManager.OnCameraOrthograficSizeChange += OnZoomChanged;
        OnZoomChanged(Camera.main.orthographicSize);
    }
    private void OnDestroy()
    {
        EventManager.OnCameraOrthograficSizeChange -= OnZoomChanged;
    }

    private void OnZoomChanged(float cameraOrthoSize)
    {
        _lineRenderer.startWidth = Utils.ScreenToWorldHeightPixel * lineWidth;
        _lineRenderer.endWidth = 0.5f * Utils.ScreenToWorldHeightPixel * lineWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.target == null)
            _lineRenderer.enabled = false;
        else
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, this.transform.position);
            //_lineRenderer.SetPosition(1, this.target.transform.position); var distance = Vector3.Distance(this.transform.position, this.target.transform.position) / 200f;
            //_lineRenderer.materials[0].mainTextureScale = new Vector3(1 / _lineRenderer.startWidth, 1, 1);
        }
    }
}
