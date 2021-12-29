using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveExtension
{
    public static TileManagerSaveInfo Save(this TileManagerBehavior manager)
    {
        TileManagerSaveInfo saveInfo = new TileManagerSaveInfo()
        {
            parent = manager.transform.parent.name,
            name = manager.name,
            rows = manager.rows,
            columns = manager.columns,
            position = manager.transform.position,
            rotation = manager.transform.rotation
        };
        foreach (var tile in manager.tiles)
        {
            if (tile == null || tile.tileInstance == null)
                continue;

            var tileSaveInfo = tile.tileInstance.GetComponent<TileBehavior>().Save();
            tileSaveInfo.row = tile.row;
            tileSaveInfo.column = tile.col;

            saveInfo.tiles.Add(tileSaveInfo);
        }
        return saveInfo;
    }
    public static void Apply(this TileManagerSaveInfo data)
    {
        var tileManager = GameObject.Find(data.name).GetComponent<TileManagerBehavior>();
        tileManager.rows = data.rows;
        tileManager.columns = data.columns;
        foreach (var tile in data.tiles)
        {
            if (tile.tileType == TileType.LOCKED || tile.tileType == TileType.BUILD)
                continue;

            GameObject tilePrefab = SaveDataMapper.Instace.mapper.FirstOrDefault(x => x.tileType == tile.tileType).tilePrefab;
            tileManager.AddTile(tile.row, tile.column, tilePrefab, true);
        }

        //TileManagerSaveInfo saveInfo = new TileManagerSaveInfo()
        //{
        //    parent = manager.transform.parent.name,
        //    name = manager.name,
        //    rows = manager.rows,
        //    columns = manager.columns,
        //    position = manager.transform.position,
        //    rotation = manager.transform.rotation
        //};
        //foreach (var tile in manager.tiles)
        //{
        //    if (tile == null || tile.tile == null)
        //        continue;

            //    saveInfo.tiles.Add(tile.tile.GetComponent<TileBehavior>().Save());
            //}
            //return saveInfo;
    }

    public static TileSaveInfo Save(this TileBehavior tile)
    {
        return new TileSaveInfo()
        {
            name = tile.name,
            tileType = tile.tileType
        };
    }

}


[System.Serializable]
public class TileSaveInfo
{
    public TileType tileType;
    public string name;
    public int row;
    public int column;

    public override string ToString()
    {
        return $"{tileType}-{name}";
    }
}

[System.Serializable]
public class TileManagerSaveInfo
{
    public string parent;
    public string name;
    public int rows = 0;
    public int columns = 0;
    public List<TileSaveInfo> tiles = new List<TileSaveInfo>();
    public Vector3 position;
    public Quaternion rotation;

    public override string ToString()
    {
        return name;
    }
}
[System.Serializable]
public class AsteroidSaveInfo
{
    public string name;
}

[System.Serializable]
public class SaveDataInfo
{
    public List<TileManagerSaveInfo> tileManager;
    public string currentLevel;

    public static SaveDataInfo GenerateSaveData()
    {
        SaveDataInfo saveInfo = new SaveDataInfo()
        {
            tileManager = SaveData.tileManagers.Select(x => x.Save()).ToList(),
            currentLevel = LevelUI.CurrentLevel
        };
        return saveInfo;
    }

    public IEnumerator Apply()
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("loading_scene", LoadSceneMode.Additive);
        while (!asyncLoadLevel.isDone)
            yield return new WaitForSeconds(1f);

        asyncLoadLevel =  SceneManager.UnloadSceneAsync("EarthDefense");
        while (!asyncLoadLevel.isDone)
            yield return new WaitForSeconds(1f);

        asyncLoadLevel = SceneManager.LoadSceneAsync("EarthDefense", LoadSceneMode.Additive);

        while (!asyncLoadLevel.isDone)
            yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(3f);

        foreach (TileManagerSaveInfo manager in tileManager)
        {
            manager.Apply();
        }
        if(!string.IsNullOrEmpty(currentLevel))
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevel, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        bool unloaded = false;
        System.Action handler = () =>
        {
            unloaded = true;
        };
        GameObject.Find("LOADING_IMAGE").GetComponent<LoadingImageBehavior>().OnLoadingCompleted += handler;
        GameObject.Find("LOADING_IMAGE").GetComponent<LoadingImageBehavior>().FadeOut();
        while (!unloaded)
            yield return new WaitForSeconds(1f);
        GameObject.Find("LOADING_IMAGE").GetComponent<LoadingImageBehavior>().OnLoadingCompleted -= handler;
        asyncLoadLevel = SceneManager.UnloadSceneAsync("loading_scene");
        while (!asyncLoadLevel.isDone)
            yield return new WaitForSeconds(1f);

        if (!string.IsNullOrEmpty(LevelUI.CurrentLevel))
        {
            asyncLoadLevel = SceneManager.UnloadSceneAsync(LevelUI.CurrentLevel);
            while (!asyncLoadLevel.isDone)
                yield return new WaitForSeconds(1f);
        }
    }
}

[System.Serializable]
public class SaveData
{
    public static SaveData _current;

    public static List<TileManagerBehavior> tileManagers = new List<TileManagerBehavior>();

    public static SaveData current
    {
        get
        {
            if (_current == null)
                _current = new SaveData();

            return _current;
        }
    }
}
