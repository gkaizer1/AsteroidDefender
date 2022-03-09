using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class IgnoreTile
{
    public float row = 0;
    public float col = 0;
}

[Serializable]
public class TileInfo
{
    public string name;
    public GameObject prefab;
    public GameObject tileInstance;
    public int row = 0;
    public int col = 0;
}

[Serializable]
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class TileManagerBehavior : MonoBehaviour
{
    public GameObject emptyTile;
    public bool isInSpace = false;

    public int rows;
    public int columns;

    public List<TileInfo> tiles = new List<TileInfo>();

    public bool Recompute = false;
    public bool GenerateGrid = false;
    public bool Clear = false;

    private void Awake()
    {
        SaveData.tileManagers.Add(this);
    }

    private void OnDestroy()
    {
        SaveData.tileManagers.Remove(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(var tile in tiles)
        {
            if (tile.tileInstance != null)
            {
                tile.tileInstance.GetComponent<TileBehavior>().TileManager = this;
            }
        }
    }

    public GameObject GetTile(float row, float column)
    {
        var existingTile = tiles.Find(x => x.col == column && x.row == row);
        return existingTile?.tileInstance;
    }

    public GameObject AddTile(float row, float col, GameObject prefab = null, bool replaceIfExists = false)
    {
        string name = $"{this.name}_row{row}_col{col}";

        string nameRegex = $@"{this.name}_row(?<ROW>(-?[0-9]\d*(\.\d+)?))_col(?<COL>(-?[0-9]\d*(\.\d+)?))";
        Regex regex = new Regex(nameRegex);

        var existingTile = tiles.Find(x => x.col == col && x.row == row);
        if (existingTile != null)
        {
            if(!replaceIfExists)
            {
                return null;
            }

            tiles.Remove(existingTile);
#if UNITY_EDITOR
            if(existingTile.tileInstance != null)
                DestroyImmediate(existingTile.tileInstance, true);
#else
            if(existingTile.tileInstance != null)
                Destroy(existingTile.tileInstance);
#endif
        }


#if UNITY_EDITOR
        GameObject newTile = PrefabUtility.InstantiatePrefab(prefab, this.transform) as GameObject;
#else
        GameObject newTile = Instantiate(prefab, this.transform) as GameObject;
#endif
        newTile.name = name;
        newTile.transform.localPosition = new Vector3(col, row, 1f);
        tiles.Add(new TileInfo()
        {
            tileInstance = newTile,
            row = (int)row,
            col = (int)col,
            prefab = prefab,
            name = name
        });

#if !UNITY_EDITOR
        Doozy.Engine.GameEventMessage.SendEvent("TILE_ADDED");
        var tileBehavior = newTile.GetComponent<TileBehavior>();
        if (tileBehavior != null)
        {
            tileBehavior.TileManager = this;
            Doozy.Engine.GameEventMessage.SendEvent($"TILE_ADDED_{tileBehavior.tileType}");
        }
#endif

        return newTile;
    }

    public void SetTile(float row, float col, GameObject tleInstance)
    {
        var existingTile = tiles.Find(x => x.col == col && x.row == row);
        if (existingTile != null)
        {
            tiles.Remove(existingTile);
            Destroy(existingTile.tileInstance);
        }

        tleInstance.transform.parent = this.transform;
        tleInstance.GetComponent<TileBehavior>().TileManager = this;
        tleInstance.name = $"{this.name}_row{row}_col{col}";
        tleInstance.transform.localPosition = new Vector3(col, row, 1f);
        tiles.Add(new TileInfo()
        {
            tileInstance = tleInstance,
            row = (int)row,
            col = (int)col,
            prefab = null,
            name = name
        });
    }

    public TileInfo GetTileInfo(GameObject tile)
    {
        return tiles.Find(x => x.tileInstance == tile);
    }


#if UNITY_EDITOR
    public void Update()
    {
        if(Clear)
        {
            Clear = false;
            foreach (var tile in tiles)
                DestroyImmediate(tile.tileInstance, true);
            tiles.Clear();
        }

        if(Recompute)
        {
            Recompute = false;
            foreach(var tile in tiles)
                DestroyImmediate(tile.tileInstance, true);
            tiles.Clear();

            var tilesCopy = new List<TileInfo>();
            tilesCopy.AddRange(tiles);
            tilesCopy.ForEach(x =>
            {
                AddTile(x.row, x.col, x.prefab, true);
            });

            if (GenerateGrid)
            {
                GenerateGrid = false;
                for (float row = -(rows / 2); row < Mathf.Ceil((float)rows / 2.0f); row++)
                {
                    for (float col = -(columns / 2); col < Mathf.Ceil((float)columns / 2.0f); col++)
                    {
                        AddTile(row, col, emptyTile);
                    }
                }
            }
        }
    }
#endif
}
