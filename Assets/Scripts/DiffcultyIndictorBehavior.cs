using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class DiffcultyIndictorBehavior : MonoBehaviour
{
    public List<Image> images = new List<Image>();
    public Sprite imageDisabled;
    public Sprite imageHighlighted;
    public int difficulty = 1;

    // Start is called before the first frame update
    void Start()
    {
        UpdateStars();
    }

    void OnValidate()
    {
        Debug.Log("Validate");
        UpdateStars();
    }

    public void UpdateStars()
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (i <= difficulty - 1)
            {
                images[i].sprite = imageHighlighted;
            }
            else
            {
                images[i].sprite = imageDisabled;
            }
        }
    }
}
