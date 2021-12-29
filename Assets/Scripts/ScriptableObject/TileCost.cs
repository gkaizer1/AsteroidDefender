using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceCost
{
    public Resource resource;
    public float Cost = 0.0f;
}

[CreateAssetMenu(menuName = "ScriptableObjects/TileCost")]
public class TileCost : ScriptableObject
{
    public GameObject Tile;
    public Sprite Icon;
    public List<ResourceCost> tileCost = new List<ResourceCost>();
}
