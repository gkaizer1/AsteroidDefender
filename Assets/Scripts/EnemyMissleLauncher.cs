using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissleLauncher : MonoBehaviour
{
    [Header("Settings")]
    public float MissleDamage = 25.0f;

    [Header("Prefabs")]
    public GameObject misslePrefab = null;

    public void Fire(Vector3 target)
    {
        var attackTarget = new GameObject();
        attackTarget.name = "attack_target_" + this.name;
        attackTarget.transform.parent = this.transform.parent;
        attackTarget.transform.position = target;

        var missle = Instantiate(misslePrefab);
        missle.transform.parent = this.transform.parent;
        missle.transform.position = this.transform.position;
        missle.transform.rotation = Quaternion.Euler(0, 0, this.transform.rotation.eulerAngles.z + 180.0f);

        missle.GetComponent<NukeBehavior>().detonationMode = NukeBehavior.DetonationMode.POINT;
        missle.GetComponent<NukeBehavior>().IgnoreCollisions = true;
        missle.GetComponent<NukeBehavior>().ExplodeAtDestination = true;
        missle.GetComponent<AutoAimBehavior>().currentTarget = attackTarget;

        missle.GetComponent<NukeBehavior>()
            .Fire()
            .OnExplosion(explosionPoint =>
            {
                GameObject.FindGameObjectWithTag("earth").GetComponent<HealthBehavior>().Health -= MissleDamage;
                Destroy(attackTarget);
            });
    }
}
