using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatusPanelBehavior : MonoBehaviour, ISelectionPanel
{
    [HideInInspector]
    public GameObject target;
    IStateMachine _stateMachine;

    [Header("UI")]
    public TextMeshProUGUI stateValueLabel;

    public void SetTarget(GameObject gameObject)
    {
        target = gameObject;
        _stateMachine = target.GetComponents<MonoBehaviour>().OfType<IStateMachine>().FirstOrDefault();
    }

    public void Update()
    {
        if(_stateMachine == null)
        {
            stateValueLabel.text = "Unknown";
            return;
        }

        string state = _stateMachine.CurrentState;
        if (stateValueLabel.text != state)
            stateValueLabel.text = state;
    }
}
