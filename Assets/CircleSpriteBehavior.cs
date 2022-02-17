using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpriteBehavior : MonoBehaviour
{
    public float Scale = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(Scale, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
