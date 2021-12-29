using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonBuild : MonoBehaviour, ISelectionPanel
{
    public GameObject BuildPanel;
    public GameObject Tile;

    public void OnBuildClicked()
    {
    }

    public void SetTarget(GameObject gameObject)
    {
        Tile = gameObject;
    }
}
