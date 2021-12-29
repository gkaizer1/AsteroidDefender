using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AstroidPanelBehavior : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI astroidCountTextPrefab;

    public void AddAstroid(Sprite sprite, int count)
    {
        GameObject imgObject = new GameObject("astroidImage");
        imgObject.transform.parent = canvas.transform;

        RectTransform trans = imgObject.AddComponent<RectTransform>();
        ///trans.anchoredPosition = new Vector2(0.5f, 0.5f);
        //trans.localPosition = new Vector3(0, 0, 0);
        //trans.position = new Vector3(0, 0, 0);
        trans.localScale = Vector3.one;
        trans.sizeDelta = new Vector2(100, 100);

        Image image = imgObject.AddComponent<Image>();
        image.sprite = sprite;
        imgObject.transform.SetParent(canvas.transform);

        var label = Instantiate<TextMeshProUGUI>(astroidCountTextPrefab, canvas.transform);
        label.text = $"x{count}";
    }
}
