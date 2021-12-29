using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuplicateSpriteMaterialBehavior : MonoBehaviour
{
    SpriteRenderer _image;

    void Awake()
    {
        _image = GetComponent<SpriteRenderer>();
        _image.material = new Material(_image.material);
        _image.material.name = $"{_image.material.name}-dupe";
    }
}
