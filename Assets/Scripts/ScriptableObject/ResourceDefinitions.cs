using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceDefinition
{
    public Resource resource;
    public string Name = "None";
    public Sprite Image;
}

[CreateAssetMenu(menuName = "ScriptableObjects/ResourceDefinitions")]
public class ResourceDefinitions : ScriptableObject
{
    public List<ResourceDefinition> resources = new List<ResourceDefinition>();
}
