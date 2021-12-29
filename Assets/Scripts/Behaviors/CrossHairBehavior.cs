using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

[DebugAttribute]
public class CrossHairBehavior : MonoBehaviour
{
    Tween tween1 = null;
    Tween tween2 = null;

    // Start is called before the first frame update
    void Start()
    {
        float animationTime = 1.0f;
        float rotation = 20.0f;
        transform.rotation = Quaternion.Euler(0, 0, -rotation);
        tween1 = transform.DORotateQuaternion(Quaternion.Euler(0, 0, rotation), animationTime).SetLoops(-1, LoopType.Yoyo);
        tween2 = transform.DOScale(1.3f, animationTime).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        tween1.Kill();
        tween2.Kill();
    }
}
