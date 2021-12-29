using Doozy.Engine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableBehavior : MonoBehaviour
{
    [Header("UI")]
    public GameObject selectionPanel;

    [Header("SelectionBehavior")]
    public bool IsSelectable = true;

    public List<string> MessagesOnSelected = new List<string>();
    public List<string> MessagesOnSelectionLost = new List<string>();

    public UnityEvent OnSelected;
    public UnityEvent OnDeSelected;

    // Start is called before the first frame update
    void Awake()
    {
        SelectionManager.OnSelectionLost += OnSelectionLost;
        SelectionManager.OnSelected += OnSelectedObject;
    }

    private void OnDestroy()
    {
        SelectionManager.OnSelectionLost -= OnSelectionLost;
        SelectionManager.OnSelected -= OnSelectedObject;
    }

    private void OnSelectedObject(GameObject obj)
    {
        if (this.gameObject != obj)
            return;

        foreach (var message in MessagesOnSelected)
            GameEventMessage.SendEvent(message, this.gameObject);

        OnSelected?.Invoke();
    }

    private void OnSelectionLost(GameObject obj)
    {
        if (this.gameObject != obj)
            return;

        foreach (var message in MessagesOnSelectionLost)
            GameEventMessage.SendEvent(message, this.gameObject);

        OnDeSelected?.Invoke();
    }
}
