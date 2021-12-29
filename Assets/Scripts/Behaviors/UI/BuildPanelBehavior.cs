using Doozy.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class BuildButtonInfo
{
    public TileType tile;
    public GameObject Button;
}

public class BuildPanelBehavior : MonoBehaviour
{
    [Header("Manager")]
    public TileManager tileManager;

    [Header("Tile Controls")]
    public List<BuildButtonInfo> buildInfo = new List<BuildButtonInfo>();

    void Start()
    {
        GameEventMessage.AddListener<GameEventMessage>(OnMessage);
        ResourceManagerBehavior.Instance.resource.ForEach(x => x.OnAmountChanged.AddListener(OnResourceChanged));
        tileManager.OnTileEnabled += OnTileEnabled;
        tileManager.OnTileDisabled += OnTileDisabled;

        Array values = Enum.GetValues(typeof(TileType));
        foreach (TileType val in values)
        {
            ToggleTile(val, tileManager.enabledTiles.Contains(val));
        }
    }

    public void OnDestroy()
    {
        tileManager.OnTileEnabled -= OnTileEnabled;
        tileManager.OnTileDisabled -= OnTileDisabled;

        ResourceManagerBehavior.Instance.resource.ForEach(x => x.OnAmountChanged.RemoveListener(OnResourceChanged));
        GameEventMessage.RemoveListener<GameEventMessage>(OnMessage);
    }

    private void OnTileEnabled(TileType tileType)
    {
        ToggleTile(tileType, true);
    }

    private void OnTileDisabled(TileType tileType)
    {
        ToggleTile(tileType, false);
    }

    public void ToggleTile(TileType tileType, bool active)
    {
        foreach(var tile in buildInfo)
        {
            if(tile.tile == tileType)
            {
                tile.Button.SetActive(active);
            }
        }
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;
    }


    public void OnResourceChanged(float newValue, float prevValue)
    {
        //buildInfo.ForEach(x =>
        //{
        //    x.button.enabled = false;
        //});
    }    

    public void SetTarget(GameObject gameObject)
    {
    }
}
