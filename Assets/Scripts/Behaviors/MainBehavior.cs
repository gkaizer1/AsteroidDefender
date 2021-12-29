using DG.Tweening;
using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        DOTween.SetTweensCapacity(20000, 5000);
        DOTween.defaultRecyclable = true;
        Time.timeScale = 1f;

        Application.targetFrameRate = 60;
    }
}
