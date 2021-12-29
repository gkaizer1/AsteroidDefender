using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AstroidExplodingState : IState<Astroid>
{
    public AstroidExplodingState(Astroid parent) : base(parent)
    {
    }

    public override string Name
    {
        get
        {
            return "Impact";
        }
    }

    public override void Init()
    {
        {
            Parent.transform.DOScale(Parent.transform.localScale.x * 0.25f, 0.5f).OnComplete(() =>
            {
                var explosion = GameObject.Instantiate(Parent.impactExplosion, GameObject.FindGameObjectWithTag("earth").transform);
                explosion.transform.position = Parent.transform.position;
                explosion.transform.localScale = Vector3.one;


                int tilesLayer = LayerMask.NameToLayer("Tiles");
                List<int> physicsMasks = new List<int>()
                {
                    1 << tilesLayer,
                };

                foreach (var mask in physicsMasks)
                {
                    var itemsInRadius = Physics2D.OverlapCircleAll(Parent.transform.position, Parent.transform.localScale.x / 2.0f, mask);
                    foreach (var item in itemsInRadius)
                    {
                        Debug.Log($"!!!! Hit Tile=[{item.gameObject.name}] !!!!");
                        var healthBehavior = item.GetComponent<HealthBehavior>();
                        if(healthBehavior != null)
                        {
                            healthBehavior.Health -= 10f;
                        }
                    }
                }

                var earth = GameObject.FindGameObjectWithTag("earth");
                float earthRadius = GameObject.FindGameObjectWithTag("earth").GetComponent<CircleCollider2D>().radius;

                Vector2 currentPostion = new Vector3(Parent.transform.position.x, Parent.transform.position.y);

                // Add the crater
                var crater = UnityEngine.Object.Instantiate(Parent.settings.crater, GameObject.FindGameObjectWithTag("earth").transform);
                crater.transform.position = Parent.transform.position;

                // Crater size depends on how close we are to the center (make smaller craters outsize)
                crater.transform.localScale = Vector3.one * Mathf.Clamp(1.0f - (currentPostion.magnitude / earthRadius), 0.3f, 1.0f);

                UnityEngine.Object.Destroy(Parent.gameObject);

                earth.GetComponent<HealthBehavior>().DoDamage(100.0f);
            });
        }

        // Clear out the indicator
        if (Parent._targetIndicatorInstance != null)
        {
            if (Parent._targetIndicatorInstance.activeSelf)
            {
                var targetSprite = Parent._targetIndicatorInstance.GetComponent<SpriteRenderer>();
                targetSprite.transform.DOScale(0.0f, 1.0f);
                targetSprite.DOFade(0.0f, 1.0f).OnComplete(() =>
                {
                    UnityEngine.Object.Destroy(Parent._targetIndicatorInstance);
                }).SetAutoKill(true);
            }
            else
                UnityEngine.Object.Destroy(Parent._targetIndicatorInstance);
        }

        Parent.rigidBody.velocity = Vector3.zero;
    }
}