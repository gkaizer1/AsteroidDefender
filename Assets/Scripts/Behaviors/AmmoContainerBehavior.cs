using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoContainerBehavior : MonoBehaviour
{
    public GameObject ammoPrefab;
    public GameObject ammoAnchor;
    public float Ammo = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        UpdateAmmo();
    }

    void UpdateAmmo()
    {
        for (int i = 0; i < Ammo; i++)
        {
            var ammoIcon = Instantiate(ammoPrefab);
            ammoIcon.name = $"ammo_{i}";
            ammoIcon.transform.parent = ammoAnchor.transform;

            float ammoGameHeight = ammoIcon.GetComponent<SpriteRenderer>().sprite.bounds.size.y * ammoIcon.transform.localScale.y;
            ammoIcon.transform.localPosition = new Vector3(0.0f, -(ammoGameHeight * i));
        }
    }
}
