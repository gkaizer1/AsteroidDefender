using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshOnAwake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }
}
