using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonRailGun : MonoBehaviour, ISelectionPanel
{
    public GameObject RailGun;

    public void OnFireRailGun()
    {
        RailGun.GetComponent<RailGunBehavior>().Fire();
    }
    public void OnSelectRailGun()
    {
        SelectionManager.SelectedGameObject = RailGun;
    }

    public void SetTarget(GameObject gameObject)
    {
        RailGun = gameObject;
    }
}
