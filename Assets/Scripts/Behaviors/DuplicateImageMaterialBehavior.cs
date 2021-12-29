using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuplicateImageMaterialBehavior : MonoBehaviour
{
    Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
        _image.material = new Material(_image.material);
        _image.material.name = $"{_image.material.name}-dupe";
    }
}
