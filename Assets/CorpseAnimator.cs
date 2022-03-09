using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CorpseAnimator : MonoBehaviour
{
    public float secondsToDie = 10.0f;
    void Start()
    {
        this.GetComponent<SpriteRenderer>().material.DOFloat(1.0f, "_GhostBlend", secondsToDie);
        this.transform.DORotate(new Vector3(0f, 0f, this.transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-30f, 30f)), secondsToDie);
        this.transform.DOScale(Vector3.one * UnityEngine.Random.Range(0.75f, 1.0f), secondsToDie);
    }
}
