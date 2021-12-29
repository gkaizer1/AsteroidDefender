using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SpaceShipAttackEarthState : IState<SpaceShipBehavior>
{
    Tweener _laserTween = null;
    public SpaceShipAttackEarthState(SpaceShipBehavior parent) : base(parent)
    {
        float laserBeamsFiring = 0;
        foreach (var laserBeam in Parent.laserBeams)
        {
            laserBeam.OnTargetReached += () =>
            {
                laserBeamsFiring++;

                // When all the laser beams reached - then start moving the target
                if (laserBeamsFiring != Parent.laserBeams.Count)
                    return;
                
                float randomRange = UnityEngine.Random.Range(1, 3);
                float randomAngle = UnityEngine.Random.Range(0, 360);

                var targetOffset = new Vector3(randomRange * Mathf.Cos(randomAngle * Mathf.Deg2Rad), randomRange * Mathf.Sin(randomAngle * Mathf.Deg2Rad));
                var damageCoRoutine = Parent.StartCoroutine(co_DoDamage(Parent.DamagePerSecond));
                _laserTween = Parent._attackTarget.transform.DOMove(Parent._attackTarget.transform.position + targetOffset, 3.0f).OnComplete(() =>
                {
                    _laserTween = null;
                    foreach (var beam in Parent.laserBeams)
                    {
                        // Stop the laser - the stop damaging the earth and fly away
                        beam.StopFiring(() =>
                        {
                            // Stop damaging the earth
                            Parent.StopCoroutine(damageCoRoutine);

                            laserBeamsFiring--;

                            // Warp away after 1 second
                            if (laserBeamsFiring == 0)
                                Parent.StartCoroutine(co_WarpAwayCounter(1f));
                        });
                    }
                }).OnKill(() => _laserTween = null);
                
            };
            laserBeam.Fire(Parent._attackTarget.transform);
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        if(_laserTween != null)
            _laserTween.Kill();
    }

    IEnumerator co_WarpAwayCounter(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        _nextState = new EnemyWrapAwayState<SpaceShipBehavior>(Parent, Parent._returnPoint).OnWarpStarted(() =>
        {
            Parent.warpTrail.SetActive(true);
        })
        .OnWarpedAway(() =>
        {
            GameObject.Destroy(Parent.gameObject);
            return null;
        });
    }
    IEnumerator co_DoDamage(float damagePerSecond)
    {

        //var earth = GameObject.FindGameObjectWithTag("earth");
        //var healthBeahvior = earth.GetComponent<HealthBehavior>(); 
        float secondPerAttack = 0.5f;
        while (true)
        {
            //healthBeahvior.DoDamage(Parent.DamagePerSecond * secondPerAttack);
            var hits = Physics2D.RaycastAll(Parent._attackTarget.transform.position, Vector2.zero);
            foreach (var hit in hits)
            {
                var healthHit = hit.collider.gameObject.GetComponent<HealthBehavior>();
                if (healthHit == null)
                    continue;
                healthHit.Health -= (Parent.DamagePerSecond * secondPerAttack);
            }
            yield return new WaitForSeconds(secondPerAttack);
        }
    }

    public override string Name
    {
        get
        {
            return "Mining";
        }
    }
}