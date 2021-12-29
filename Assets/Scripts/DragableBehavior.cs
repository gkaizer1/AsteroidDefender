using Doozy.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragableBehavior : MonoBehaviour
{
    public UnityEvent<GameObject, Vector3, Vector3> OnMouseDragDelta;
    public UnityEvent<GameObject, Vector3> OnMouseDragStart;
    public UnityEvent<GameObject, Vector3, Vector3> OnMouseDragEnd;

    public Color enteredColor;

    [Header("Children")]
    public List<SpriteRenderer> childrenToColorOnEnter = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, Color> _previousColors = new Dictionary<SpriteRenderer, Color>();

    Vector3 mouseStart;
    bool _dragging = false;
    Vector3 _initialSize;

    private void Start()
    {
        _initialSize = this.transform.localScale;
    }

    private void OnMouseDown()
    {
        if (Utils.IsOverUIElement())
        {
            return;
        }

        GameEventMessage.SendEvent("LOCK_CAMERA");
        mouseStart = Input.mousePosition;
        _dragging = false;
    }

    private void OnMouseUp()
    {
        if (Utils.IsOverUIElement())
        {
            ResetColors();
            return;
        }

        GameEventMessage.SendEvent("UNLOCK_CAMERA");
        if (_dragging)
        {
            OnMouseDragEnd?.Invoke(gameObject, mouseStart, Input.mousePosition);
            _dragging = false;
        }
        ResetColors();
    }
    
    private void OnMouseDrag()
    {
        if (Utils.IsOverUIElement())
        {
            ResetColors();
            return;
        }

        if (!_dragging)
        {
            _dragging = true;
            OnMouseDragStart?.Invoke(gameObject, mouseStart);
        }

        OnMouseDragDelta?.Invoke(gameObject, mouseStart, Input.mousePosition);
    }

    public void OnMouseEnter()
    {
        if (Utils.IsOverUIElement())
        {
            ResetColors();
            return;
        }

        if (_previousColors.Count <= 0)
            ColorizeIndicators();
    }

    public void ColorizeIndicators()
    {
        foreach (var sprite in childrenToColorOnEnter)
        {
            _previousColors[sprite] = sprite.color;
            sprite.color = enteredColor;
        }
        _initialSize = this.transform.localScale;
        this.transform.localScale = this.transform.localScale * 1.2f;
    }

    public void OnMouseExit()
    {
        if (!_dragging)
            ResetColors();
    }

    public void ResetColors()
    {
        foreach (var sprite in childrenToColorOnEnter)
        {
            if (_previousColors.ContainsKey(sprite))
                sprite.color = _previousColors[sprite];
        }
         this.transform.localScale = _initialSize;
        _previousColors.Clear();
    }
}
