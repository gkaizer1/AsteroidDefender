using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileBehavior : MonoBehaviour
{
    public int tileSize = 1;
    public TileManagerBehavior TileManager;
    public TileType tileType;
    public bool IsInSpace = false;
}
