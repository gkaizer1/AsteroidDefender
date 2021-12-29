using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class NukeLostState : IState<NukeBehavior>
{
    public Rigidbody2D rigidBody;

    public NukeLostState(NukeBehavior parent) : base(parent)
    {
    }

    public override string Name
    {
        get
        {
            return "Out of fuel";
        }
    }

    public override void Init()
    {
        rigidBody = Parent.GetComponent<Rigidbody2D>();

        float destroyTime = 2.0f;

        DOTween.To(
            () => rigidBody.velocity,
            x => rigidBody.velocity = x,
            new Vector2(0f, 0f),
            destroyTime / 2.0f);

        DOTween.To(
            () => rigidBody.rotation,
            x => rigidBody.rotation = x,
            180,
            destroyTime / 2.0f)
        .OnComplete(() =>
        {
            // Make object disappear after stopping
            Parent.transform.DOScale(0.0f, destroyTime / 2.0f).OnComplete(() =>
            {
                GameObject.Destroy(Parent.gameObject);
            });
        });
    }

    public override IState<NukeBehavior> Update()
    {
        return this;
    }
}
