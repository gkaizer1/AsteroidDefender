using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Weapons/VulcanCannonSettings")]
public class VulcanCannonSettings : ScriptableObject
{
    [Header("Weapon")]
    public float maxBullets = 10.0f;
    public float reloadTime = 0.1f;
    public float InitialHealth = 100.0f;
    public float Damage = 1.0f;
    public float rotationSpeed = 120.0f;
    public float powerRequiredToFire = 1.0f;
}
