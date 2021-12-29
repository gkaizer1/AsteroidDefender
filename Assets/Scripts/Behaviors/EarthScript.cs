using Doozy.Engine;
using Doozy.Engine.Nody;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EarthScript : MonoBehaviour
{
    [Header("Children")]
    public GameObject shattredSprite;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            this.GetComponent<HealthBehavior>().Health -= 1000;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.GetComponent<HealthBehavior>().Health += 1000;
        }
    }

    public void OnHealthChanged(float current, float max)
    {

        float value = EasingFunction.EaseInQuad(0.0f, 1.0f, Mathf.Clamp(current / max, 0f, 1.0f));
        shattredSprite.GetComponent<SpriteRenderer>().material.SetFloat("_FadeAmount", value);
        if(current == 0)
        {
            GraphController controller = GraphController.Get("Graph Controller");
            controller.GoToNodeByName("GAME_OVER");
        }
    }
}
