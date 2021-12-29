using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicatorBehavior : MonoBehaviour
{
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

    public Color PrecentageColor(float precentage)
    {
        Color color = Color.green;
        if (precentage > 0.80)
        {
            color = Color.green;
        }
        else if (precentage > 0.5)
        {
            color = Color.yellow;
        }
        else if (precentage > 0.1)
        {
            color = Color.red;
        }
        else
        {
            color = Color.red;
        }
        return color;
    }

    public void OnHealthChanged(float current, float max)
    {
        if(HideWhenFull && current == max)
        {
            foreach (var healthLight in healthLights)
            {
                healthLight.SetActive(false);
            }
            return;
        }

        Color color = Color.green;
        float lifePrecentage = current / max;
        float damagePrecent = 1.0f - lifePrecentage;
        if (lifePrecentage > 0.80)
        {
            color = Color.green;
            healthLights[0].SetActive(true);
            healthLights[0].GetComponent<SpriteRenderer>().color = Color.green;
            healthLights[1].SetActive(true);
            healthLights[1].GetComponent<SpriteRenderer>().color = Color.green;
            healthLights[2].SetActive(true);
            healthLights[2].GetComponent<SpriteRenderer>().color = PrecentageColor((lifePrecentage - 0.2f) / 0.8f);
        }
        else if (lifePrecentage > 0.5)
        {
            color = Color.yellow;
            healthLights[0].SetActive(true);
            healthLights[0].GetComponent<SpriteRenderer>().color = Color.green;
            healthLights[1].SetActive(true);
            healthLights[1].GetComponent<SpriteRenderer>().color = PrecentageColor((lifePrecentage - 0.5f) / 0.5f);
            healthLights[2].SetActive(false);
        }
        else if (lifePrecentage > 0.1)
        {
            color = Color.red;
            healthLights[0].SetActive(true);
            healthLights[0].GetComponent<SpriteRenderer>().color = PrecentageColor((lifePrecentage - 0.9f) / 0.1f);
            healthLights[1].SetActive(false);
            healthLights[2].SetActive(false);
        }
        else
        {
            color = Color.red;
            healthLights[0].SetActive(false);
            healthLights[1].SetActive(false);
            healthLights[2].SetActive(false);
        }

        foreach (var healthLight in healthLights)
        {
            //healthLight.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
