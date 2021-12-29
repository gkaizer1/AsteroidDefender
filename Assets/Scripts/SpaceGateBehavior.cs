using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class SpaceGateBehavior : MonoBehaviour
{
    public GameObject portal;
    public UnityEvent OnGateSpawned;
    public UnityEvent OnGateClosed;

    Vector3 _position = Vector3.zero;
    bool _warpedIn = false;

    private void Start()
    {
        var sprite = this.GetComponent<SpriteRenderer>();
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.0f);
        sprite.DOFade(1.0f, 2.0f);
        _position = this.transform.localPosition;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, 100f, this.transform.localPosition.z);
    }

    public void WarpInGate()
    {
        if (_warpedIn)
            return;

        float destY = _position.y;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, 100f, this.transform.localPosition.z);
        this.transform.DOLocalMoveY(destY, 2.0f).OnComplete(() =>
        {
            this.GetComponent<Animator>().SetTrigger("spawn");
            OnGateSpawned?.Invoke();
        });
        _warpedIn = true;
    }

    public void WarpOut()
    {
        if (!_warpedIn)
            return;

        this.transform.DOLocalMoveY(100f, 2.0f).OnComplete(() =>
        {
            _warpedIn = false;
        });
    }

    public void OpenSpawnGate()
    {
        if (!_warpedIn)
            WarpInGate();

        portal.SetActive(true);
        portal.transform.localScale = Vector3.zero;
        portal.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 2.0f);
    }

    public void CloseSpawnGate()
    {
        portal.transform.DOScale(Vector3.zero, 2.0f).OnComplete(() => OnGateClosed?.Invoke());
    }

    public void ZoomToGate()
    {
        ZoomBehavior.Instance.ZoomToPoint(this.transform.position, 3.0f);
    }
}
