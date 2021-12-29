using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="GameState", menuName ="State/Game")]
public class GameState : ScriptableObject
{
    public float GameTime;

    public GameObject SelectedTarget;


    public void SelectObject(GameObject gameObject)
    {

    }
}
