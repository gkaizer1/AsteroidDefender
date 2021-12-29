using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CircleFillerBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Image>().fillAmount = 0.0f;
    }

    public void SetFill(float fill)
    {
        this.GetComponent<Image>().DOFillAmount(fill, 1.0f);
    }
}
