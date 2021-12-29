using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBaseBehavior : MonoBehaviour
{
    private bool _isSelected = false;

    // Start is called before the first frame update
    void Awake()
    {
        SelectionManager.OnSelected += OnSelectionChanged;
    }

    void OnDestroy()
    {
        SelectionManager.OnSelected -= OnSelectionChanged;
    }

    public void OnSelectionChanged(GameObject obj)
    {
        Selected = this.gameObject.IsParent(obj);
    }

    public bool Selected
    {
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            if(_isSelected)
            {
                var spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.material.EnableKeyword("HITEFFECT_ON");
                spriteRenderer.material.SetColor("_HitEffectColor", Color.green);
                spriteRenderer.material.DOFloat(0.75f, "_HitEffectBlend", 0.25f).SetLoops(-1, LoopType.Yoyo).OnKill(() =>
                {
                    spriteRenderer.material.SetFloat("_HitEffectBlend", 0.0f);
                    spriteRenderer.material.DisableKeyword("HITEFFECT_ON");
                });
            }
            else
            {
                var spriteRenderer = GetComponent<SpriteRenderer>();
                DOTween.TweensByTarget(spriteRenderer.material, true)?.ForEach(x => x.Kill(true));
            }
        }
        get
        {
            return _isSelected;
        }
    }
}
