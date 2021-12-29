using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonNuclearSilo : MonoBehaviour, ISelectionPanel
{
    public GameObject Silo;

    public void OnFireNuke()
    {
        Silo.GetComponent<NuclearSiloBehavior>().Fire();
    }

    public void OnSelectSilo()
    {
        SelectionManager.SelectedGameObject = Silo;
    }

    public void SetTarget(GameObject gameObject)
    {
        Silo = gameObject;
    }
}
