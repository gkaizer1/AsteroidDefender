using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDeathBehavior : MonoBehaviour
{
    public GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<HealthBehavior>().OnHealthChanged.AddListener(OnHealthChanged);
    }

    public void OnHealthChanged(float current, float max)
    {
        if (current > 0)
            return;

        if (explosionPrefab != null)
        {
            var explosion = Instantiate(explosionPrefab, this.transform.parent);
            explosion.name = "explosion_" + this.name;
            explosion.transform.parent = this.transform.parent;
            explosion.transform.position = this.transform.position;
        }
    }
}
