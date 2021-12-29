using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingImageBehavior : MonoBehaviour
{
    public event System.Action OnLoadingCompleted;
    public List<GameObject> objectsToDisable;

    public void FadeOut()
    {
        foreach(var ob in objectsToDisable)
        {
            ob.SetActive(false);
        }
        this.GetComponent<Image>().DOFillAmount(0, 1.0f).OnComplete(() => OnLoadingCompleted?.Invoke());
    }
}
