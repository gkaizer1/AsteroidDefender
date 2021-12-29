using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectionListenerBehavior : MonoBehaviour
{
    public UnityEvent OnSlectionChanged;
    public UnityEvent OnNoSelection;

    void Start()
    {
        SelectionManager.OnSelectedObjectChanged += OnSelectionChanged;
    }

    private void OnDestroy()
    {
        SelectionManager.OnSelectedObjectChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged(GameObject previous, GameObject current)
    {
        if (current != null)
            OnSlectionChanged.Invoke();
        if (current == null)
            OnNoSelection.Invoke();
    }
}
