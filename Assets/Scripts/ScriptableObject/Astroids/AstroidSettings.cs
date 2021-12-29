using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Astroid/AstroidSettings")]
public class AstroidSettings : ScriptableObject
{
    [Header("AstroidSettings")]
    public float StartSpeed = 1.0f;
    public float Acceleration = 0.05f;
    public string AstroidType = "Astroid";
    public float InitialHealth = 100.0f;
    public float Damage = 1.0f;

    [Range(0, 10)]
    public float MinScale = 0.5f;
    [Range(0, 10)]
    public float MaxScale = 1.0f;

    [Header("Prefabs")]
    public GameObject crater;
}
