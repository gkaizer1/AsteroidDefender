using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LevelTileSettings
{
    public string name;
    public int level;
    public string TileManagerName;
    public int row;
    public int column;
}

[CreateAssetMenu(menuName = "ScriptableObjects/GlobalGameSettings")]
public class GlobalGameSettings : ScriptableObject
{
    [Header("Prefabs")]
    public GameObject unlockedTilePrefab;
    public GameObject EmptySatellitePrefab;

    public List<LevelTileSettings> levelTiles = new List<LevelTileSettings>();

    public static GlobalGameSettings Instance;

    public GlobalGameSettings()
    {
        Instance = this;
    }

    public void LevelStart(int level)
    {
        UnlockPreviousLevelTiles(level);
    }

    /*
     * Unlock tiles that should've been unlocked
     */
    public void UnlockPreviousLevelTiles(int level)
    {
        foreach (var levelTile in levelTiles.Where(x => x.level < level))
        {
            TileManagerBehavior tileManager = GameObject.Find(levelTile.TileManagerName).GetComponent<TileManagerBehavior>();
            if (tileManager == null)
            {
                Debug.LogError($"Failed to file TileManager=[{tileManager}]");
                continue;
            }
            // Make sure the tile type is of type [LOCKED] - DO NOT OVERRIDE existing tiles
            var existingTile = tileManager.GetTile(levelTile.row, levelTile.column);
            if (existingTile != null)
            {
                var tileBehavior = existingTile.GetComponent<TileBehavior>();
                if (tileBehavior != null && tileBehavior.tileType != TileType.LOCKED)
                    continue;
            }

            var tile = tileManager.AddTile(
                            levelTile.row,
                            levelTile.column,
                            unlockedTilePrefab,
                            true /* overwrite-existing */);
        }
    }
}
