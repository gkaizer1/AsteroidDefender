using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScreenBehavior : MonoBehaviour
{
    public Vector3 position = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Camera.main.WorldToScreenPoint(position);
    }

    public string Text
    {
        set
        {
            this.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = value;
        }
    }

    public float Rotation
    {
        set
        {
            this.transform.rotation = Quaternion.Euler(0, 0, value);
        }
    }

    public Sprite Icon
    {
        set
        {
            this.GetComponentInChildren<Image>().sprite = value;
        }
    }
}
