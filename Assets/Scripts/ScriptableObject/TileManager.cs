using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    BUILD,
    LASER,
    RAIL_GUN,
    NUCLEAR_SILO,
    VULCAN,
    SHIELD,
    POWER,
    SHUTTLE,
    SATELLITE_VULCAN,
    COMMERICAL_DISTRICT,
    INDUSTRIAL_DISTRICT,
    LOCKED,
    CONNECTOR,
    NONE
}

[CreateAssetMenu(menuName = "ScriptableObjects/TileManager")]
public class TileManager : ScriptableObject
{
    public List<TileType> enabledTiles = new List<TileType>();

    public event System.Action<TileType> OnTileEnabled;
    public event System.Action<TileType> OnTileDisabled;

    public void EnableTile(TileType tileType)
    {
        if (!enabledTiles.Contains(tileType))
            enabledTiles.Add(tileType);
        OnTileEnabled?.Invoke(tileType);
    }

    public void DisableTile(TileType tileType)
    {
        if (enabledTiles.Contains(tileType))
            enabledTiles.Remove(tileType);
        OnTileDisabled?.Invoke(tileType);
    }
}
