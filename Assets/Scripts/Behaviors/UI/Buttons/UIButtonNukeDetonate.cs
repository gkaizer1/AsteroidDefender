using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonNukeDetonate : MonoBehaviour, ISelectionPanel
{
    public GameObject Nuke;

    public void OnDetonateClicked()
    {
    }

    public void SetTarget(GameObject gameObject)
    {
        Nuke = gameObject;
    }
}
