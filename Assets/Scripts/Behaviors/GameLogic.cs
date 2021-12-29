using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;

public class GameLogic : MonoBehaviour
{
    public GameState gameState;
    public GameObject ClickHandler;

    private void OnGameObjectSeleted(GameObject obj)
    {
    }

    // Update is called once per frame
    void Update()
    {
        gameState.GameTime += Time.deltaTime;
    }
}
