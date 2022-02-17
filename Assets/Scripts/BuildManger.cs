using Doozy.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BuildItem
{
    public string Name;
    public GameObject prefab;
    public GameObject buildPrefab = null;
}

[Serializable]
public class BuildSatelliteInfo
{
    public string Name;
    public GameObject SatellitePrefab;
}

public class BuildManger : MonoBehaviour
{
    public GameObject satellitePrefab;
    public List<BuildItem> buildItems = new List<BuildItem>();
    public List<BuildSatelliteInfo> satellites = new List<BuildSatelliteInfo>();

    public static GameObject buildTile = null;

    [Header("Prefabs")]
    public GameObject underContructionPrefab = null;
    public GameObject unlockedTilePrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        Message.AddListener<GameEventMessage>(OnMessage);
        SelectionManager.OnSelectedObjectChanged += OnSelectionChanged;
    }

    private void OnDestroy()
    {
        Message.RemoveListener<GameEventMessage>(OnMessage);
        SelectionManager.OnSelectedObjectChanged -= OnSelectionChanged;
    }

    private void OnDisable()
    {
        //Stop listening for game events
        Message.RemoveListener<GameEventMessage>(OnMessage);
    }
    private void OnSelectionChanged(GameObject previous, GameObject current)
    {
        if (current != null)
        {
            buildTile = current;            
        }
        else
        {
            buildTile = null;
        }
    }
    private void BuildSatellite(BuildSatelliteInfo buildItem)
    {
        GameObject underContruction = Instantiate(satellitePrefab);
        
        var construcatbleBehavior = underContruction.GetComponent<ConstructableBehavior>();
        construcatbleBehavior.constructionTime = 5.0f;
        construcatbleBehavior.ObjectToConstructPrefab = buildItem.SatellitePrefab;
        construcatbleBehavior.IsBuilding = false;

        var polarCoordinate = underContruction.GetComponent<PolarCoordinateTransform>();
        if (polarCoordinate)
        {
            polarCoordinate.Angle = 180.0f;
            polarCoordinate.Radius = 10.0f;
        }

        ShuttleMission_BuildSatellite mission = new ShuttleMission_BuildSatellite(underContruction, construcatbleBehavior);
        ShuttleMissionManager.Instance.ScheduleMission(mission);
    }

    private void BuildTile(GameObject tile, BuildItem buildItem)
    {
        var tileManager = tile.GetComponent<TileBehavior>().TileManager;
        bool isInSpace = false;
        if (tile.GetComponent<TileBehavior>() != null && tile.GetComponent<TileBehavior>().TileManager != null)        
            isInSpace = tile.GetComponent<TileBehavior>().TileManager.isInSpace;        

        var buildPrefab = buildItem.buildPrefab != null ? buildItem.buildPrefab : underContructionPrefab;
        GameObject underContruction = null;

        if (tileManager != null)
        {
            var tileInfo = tileManager.GetTileInfo(tile);
            underContruction = tileManager.AddTile(tileInfo.row, tileInfo.col, buildPrefab, true);
        }
        else
        {
            underContruction = Instantiate(buildPrefab);
            underContruction.transform.parent = tile.transform.parent;
            underContruction.transform.localPosition = tile.transform.localPosition;
            underContruction.transform.localRotation = tile.transform.localRotation;
        }

        var underConstructionBehavior = underContruction.GetComponent<TileUnderConstructionBehavior>();
        if(underConstructionBehavior)
        { 
            underConstructionBehavior.constructionTime = 5.0f;
            underConstructionBehavior.TileToBuild = buildItem.prefab;
        }

        // Delete the tile to prevent re-scheduling build
        if (tile != null)
            Destroy(tile);

        // Clear out selection
        SelectionManager.SelectedGameObject = null;
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null) 
            return;

        if (message.EventName.StartsWith("BUILD_MANAGER.BUILD"))
        {
            string buildItem = message.EventName.Replace("BUILD_MANAGER.BUILD.", "");
            var buildItemInfo = buildItems.FirstOrDefault(x => x.Name == buildItem);
            if (buildItemInfo == null)
            {
                Debug.LogError($"Failed to find build item [{buildItem}]");
                return;
            }
            BuildTile(buildTile, buildItemInfo);
            GameEventMessage.SendEvent("HIDE_BUILD_PANEL");
        }
        if (message.EventName.StartsWith("BUILD_MANAGER.SATELLITE_BUILD"))
        {
            string buildItem = message.EventName.Replace("BUILD_MANAGER.SATELLITE_BUILD.", "");
            var buildItemInfo = this.satellites.FirstOrDefault(x => x.Name == buildItem);
            if (buildItemInfo == null)
            {
                Debug.LogError($"Failed to find satellite build item [{buildItem}]");
                return;
            }

            BuildSatellite(buildItemInfo);
            GameEventMessage.SendEvent("HIDE_BUILD_PANEL");
        }
    }
}
