using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Doozy.Engine;

public class HideablePanel : MonoBehaviour
{
    private Animator _animator;

    public string ShowMessage;
    public List<string> HideMessages;
    public string HideMessage;

    public Vector2 ShowPostion;
    public Vector2 HidePosition;

    public float animationTime = 0.5f;

    public enum SLIDE_ANIMATON
    {
        SLIDE_LEFT,
        SLIDE_RIGHT,
        NONE
    }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public SLIDE_ANIMATON animation = SLIDE_ANIMATON.NONE;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    public RectTransform _rectTransform;

    // Start is called before the first frame update
    void Awake()
    {
        GameEventMessage.AddListener<GameEventMessage>(OnMessage);

        _animator = _animator ? _animator : this.GetComponent<Animator>();
        _rectTransform = _rectTransform ? _rectTransform : GetComponent<RectTransform>();
    }

    public void OnDestroy()
    {
        GameEventMessage.RemoveListener<GameEventMessage>(OnMessage);
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        if (message.EventName == ShowMessage)
        {
            if(_animator != null)
                _animator.SetTrigger("SHOW_PANEL");

            if(animation == SLIDE_ANIMATON.SLIDE_LEFT)
            {
                DOTween.To(
                    () => _rectTransform.anchoredPosition,
                    x => _rectTransform.anchoredPosition = x,
                    new Vector2(-5, _rectTransform.anchoredPosition.y),
                    animationTime);
            }
            if (animation == SLIDE_ANIMATON.SLIDE_RIGHT)
            {
                DOTween.To(
                    () => _rectTransform.anchoredPosition,
                    x => _rectTransform.anchoredPosition = x,
                    new Vector2(5, _rectTransform.anchoredPosition.y),
                    animationTime);
            }
        }

        if (message.EventName == HideMessage || HideMessages.Contains(message.EventName))
        {
            if (_animator != null)
                _animator.SetTrigger("HIDE_PANEL");

            if (animation == SLIDE_ANIMATON.SLIDE_LEFT)
            {
                DOTween.To(
                    () => _rectTransform.anchoredPosition,
                    x => _rectTransform.anchoredPosition = x,
                    new Vector2(1000, _rectTransform.anchoredPosition.y),
                    animationTime);
            }
            if (animation == SLIDE_ANIMATON.SLIDE_RIGHT)
            {
                DOTween.To(
                    () => _rectTransform.anchoredPosition,
                    x => _rectTransform.anchoredPosition = x,
                    new Vector2(-1000, _rectTransform.anchoredPosition.y),
                    animationTime);
            }
        }
    }
}
