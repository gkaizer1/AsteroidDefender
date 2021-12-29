using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IClickHander
{
    void OnClicked();
}
public interface IScreenClickedHandler
{
    void OnScreenClicked(Vector2 worldPoint);
}

public class ClickHandler : MonoBehaviour
{
    Vector3 _mouseStart = Vector3.zero;
    public Cinemachine.CinemachineVirtualCamera zoomCamera;

    private void Awake()
    {
        SelectionManager.OnSelected += OnSelectionChanged;
    }

    private void OnSelectionChanged(GameObject obj)
    {
        if (zoomCamera == null)
            return;

        SatelliteBehavior satellite = null;
        if(obj != null)
            satellite = obj.GetComponentInChildren<SatelliteBehavior>();

        if (obj != null && satellite == null)
        {
            zoomCamera.enabled = true;
            zoomCamera.Follow = obj.transform;
        }
        else
        {
            zoomCamera.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouseStart = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if ((_mouseStart - Input.mousePosition).magnitude > 2.0f)
                return;
            
            if (Utils.IsOverUIElement())
            {
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            var hits = Physics2D.OverlapCircleAll(mousePos2D, 1f).ToList();
            GameObject selectedObject = null;
            float minDistance = float.MaxValue;
            foreach (var hit in hits)
            {
                if (hit.gameObject == null)
                    return;

                try
                {
                    var selectableBehavior = hit.gameObject.GetComponent<SelectableBehavior>();
                    if (selectableBehavior != null && selectableBehavior.IsSelectable)
                    {
                        float distance = Vector2.Distance(mousePos2D, hit.gameObject.transform.position);
                        if (distance < minDistance)
                        {
                            selectedObject = hit.gameObject;
                            minDistance = distance;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            };
            SelectionManager.SelectedGameObject = selectedObject;
        }
    }

    public void CancelSelection()
    {
        SelectionManager.SelectedGameObject = null;
    }
}
