using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
using UnityEditor;
#endif

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class HealthIndicatorBehavior : MonoBehaviour
{
    [Range(0, 100)]
    public int lights = 3;
    [Range(0, 1)]
    public float spacing = 0.075f;
    public Sprite healthLightSprite = null;
    public List<GameObject> healthLights = new List<GameObject>();
    public bool HideWhenFull = false;

    private void Start()
    {
        var health = this.GetComponentInParent<HealthBehavior>();
        if (health)
            health.OnHealthChanged.AddListener(OnHealthChanged);

        if (HideWhenFull)
        {
            foreach (var healthLight in healthLights)
            {
                healthLight.SetActive(false);
            }
        }
    }

    public void OnHealthChanged(float current, float max)
    {
        if(HideWhenFull && current != max)
        {
            HideWhenFull = false;
            foreach (var healthLight in healthLights)
            {
                healthLight.SetActive(true);
            }
        }

        Color color = Color.green;
        float lifePrecentage = current / max;
        float damagePrecent = 1.0f - lifePrecentage;

        float lifePerBlock = max / lights;
        for (int i = 0; i < lights; i++)
        {
            var currentBlockSpriteRenderer = healthLights[healthLights.Count - i - 1].GetComponent<SpriteRenderer>();
            float currentBlockMinLife = lifePerBlock * i;
            float currentBlockMaxLife = lifePerBlock * (i + 1);
            if (current > currentBlockMaxLife)
            {
                currentBlockSpriteRenderer.enabled = true;
                healthLights[healthLights.Count - i - 1].GetComponent<SpriteRenderer>().color = Color.green;
            }
            else if(lifePrecentage > currentBlockMinLife)
            {
                currentBlockSpriteRenderer.enabled = true;
                // How much life is left in this block
                float precentageLeftInBlock = (current - currentBlockMinLife) / lifePerBlock;
                if (precentageLeftInBlock > 0.9f)
                    healthLights[healthLights.Count - i - 1].GetComponent<SpriteRenderer>().color = Color.green;
                else if (precentageLeftInBlock > 0.4f)
                    healthLights[healthLights.Count - i - 1].GetComponent<SpriteRenderer>().color = Color.yellow;
                else
                    healthLights[healthLights.Count - i - 1].GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                currentBlockSpriteRenderer.enabled = false;
            }
        }
    }

#if UNITY_EDITOR

    public bool _update = false;
    public void Update()
    {
        if (!_update)
            return;
        
        _update = false;
        foreach (Transform light in this.transform)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(light.gameObject, true);
            };
        }
        
        healthLights.Clear();

        for (int i = 0; i < lights; i++)
        {
            GameObject go = new GameObject($"light_{i}");
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = this.healthLightSprite;
            renderer.color = Color.green;
            go.transform.parent = this.gameObject.transform;
            go.transform.localPosition = new Vector3(go.transform.position.x + ((float)i) * spacing, 0f, 0f);
            healthLights.Add(go);
        }
    }
#endif
}
