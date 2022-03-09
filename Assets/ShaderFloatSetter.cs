using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderFloatSetter : MonoBehaviour
{
    public string ShaderProperty;
    public float value;
    float _previousValue;

    public Material material;
    public SpriteRenderer sprite;

    void Start()
    {
        if (sprite != null && sprite.material != null)
            material = sprite.material;

        SetFloat(value);
    }

    public void Update()
    {
        SetFloat(value);
    }

    public void SetFloat(float newValue)
    {
        if (newValue == _previousValue)
            return;

        material.SetFloat(ShaderProperty, newValue);

        _previousValue = newValue;
        value = newValue;
    }
}