using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MineralIndicatorBehavior : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;

    float timeToDie = 100.0f;
    Tween _fadeTween = null;
    public bool IsAvailable = true;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timeToDie -= Time.deltaTime;
        if(timeToDie < 0 && _fadeTween == null)
        {
            IsAvailable = false;
            _fadeTween = _spriteRenderer.DOFade(0.0f, 2.0f).OnComplete(() => Destroy(this.gameObject));
        }
    }
}
