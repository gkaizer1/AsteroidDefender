using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShaderColorSetterBehavior : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color color;
    Color _prevoiousColor;

    public string hdrColorProperty;

    public Material material;

    public SpriteRenderer sprite;

    void Start()
    {
        if (sprite != null && sprite.material != null)
            material = sprite.material;

        UpdateColor(color);
    }

    public void Update()
    {
        UpdateColor(color);
    }

    public void UpdateColor(Color color)
    {
        if (color == _prevoiousColor)
            return;

        material.SetColor(hdrColorProperty, color);
        _prevoiousColor = color;
    }
}
