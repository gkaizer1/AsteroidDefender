using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBehavior : MonoBehaviour
{
    private TextMeshProUGUI textobject;
    private GameObject _uiTextHolder;

    public GameObject anchorPoint;

    public bool StartHidden = false;

    // Start is called before the first frame update
    void Awake()
    {
        var canvas = GameObject.FindGameObjectsWithTag("text_canvas")[0] as GameObject;
        _uiTextHolder = Instantiate(Resources.Load("UI_TEXT_HOLDER"), canvas.transform) as GameObject;
        _uiTextHolder.name = $"text_holder_{this.name}";
        _uiTextHolder.SetActive(!StartHidden);

        textobject = _uiTextHolder.GetComponentInChildren<TextMeshProUGUI>();
        textobject.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (anchorPoint != null)
        {
            _uiTextHolder.transform.position = Camera.main.WorldToScreenPoint(anchorPoint.transform.position);
        }
        else
        {
            _uiTextHolder.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
        }
    }

    public void Show(bool show)
    {
        _uiTextHolder.SetActive(show);
    }

    public string Text
    {
        set
        {
            textobject.text = value;
        }
    }

    private void OnDestroy()
    {
        Destroy(_uiTextHolder);
    }
}
