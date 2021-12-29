using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileMapper
{
    public TileType tileType;
    public GameObject tilePrefab;
}

public class SaveDataMapper : MonoBehaviour
{
    public List<TileMapper> mapper;
    public static SaveDataMapper Instace;

    private void Awake()
    {
        Instace = this;
    }
}
