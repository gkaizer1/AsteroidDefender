using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyManager : MonoBehaviour
{
    public static List<GameObject> Enemies = new List<GameObject>();
    public static EnemyManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }
}
