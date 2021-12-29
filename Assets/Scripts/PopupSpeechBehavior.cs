using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Engine.UI;
using Febucci.UI;
using System;
using UnityEngine.UI;

public class PopupSpeechBehavior : MonoBehaviour
{
    public TextMeshProUGUI _titleComponent;
    public TextMeshProUGUI _textComponent;

    public event Action OnDestroyed;
    public string Text;
    public string Title;

    public float _originalTimeScale = 1.0f;
    public Button closeButton = null;

    private void Awake()
    {
        _originalTimeScale = Time.timeScale;
        //Time.timeScale = 0.1f;
        _textComponent.text = "";
        _titleComponent.text = "";

        HideCloseButton();
    }

    private void Start()
    {
        _titleComponent.text = Title;
    }

    public void HideCloseButton()
    {
        closeButton.gameObject.SetActive(false);
    }

    public void ShowCloseButton()
    {
        closeButton.gameObject.SetActive(true);
    }

    IEnumerator OnStart()
    {
        _titleComponent.text = Title;
        _textComponent.text = "";
        yield return new WaitForSecondsRealtime(0.1f);
        _textComponent.GetComponent<TextAnimatorPlayer>().ShowText(Text);
    }
    public void Update()
    {
        // Mouse button down - skip the speech
        if (Input.GetMouseButtonDown(0))
        {
            _textComponent.GetComponent<TextAnimatorPlayer>().SkipTypewriter();
        }
    }

    public void ShowText()
    {
        StartCoroutine(OnStart());
    }

    private void OnDestroy()
    {
        //Time.timeScale = _originalTimeScale;
        OnDestroyed?.Invoke();
    }

    public void Close()
    {
        this.GetComponent<UIPopup>().Hide();
    }

    public static PopupSpeechBehavior CreateSpeechDialog(string title, string text)
    {
        var popup = UIPopup.GetPopup("Popup - Speech");
        var speechDialog = popup.Canvas.GetComponentInChildren<PopupSpeechBehavior>();

        if (speechDialog != null)
        {
            speechDialog.Title = title;
            speechDialog.Text = text;
        }
        UIPopupManager.AddToQueue(popup);
        return speechDialog;
    }
}