using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class RepairButtonBehavior : MonoBehaviour
{
    private GameObject _previousGameObject;

    [Header("Children")]
    public List<Image> imagesToFade = new List<Image>();
    public TextMeshProUGUI labelToFade = null;
    public float animationTimeSeconds = 0.25f;

    public void Awake()
    {
        SelectionManager.OnSelectedObjectChanged += OnSelectedObjectChanged;
    }

    private void OnDestroy()
    {
        SelectionManager.OnSelectedObjectChanged -= OnSelectedObjectChanged;
    }

    private void OnSelectedObjectChanged(GameObject previous, GameObject current)
    {
        if (_previousGameObject != null)
        {
            HealthBehavior previousHealthBeahvior = _previousGameObject.GetComponent<HealthBehavior>();
            if (previousHealthBeahvior != null)
            {
                previousHealthBeahvior.OnHealthChanged.RemoveListener(OnHealthChanged);
            }
        }

        if (current != null)
        {
            HealthBehavior healthBeahvior = current.GetComponent<HealthBehavior>();
            if (healthBeahvior != null)
            {
                _previousGameObject = current;
                healthBeahvior.OnHealthChanged.AddListener(OnHealthChanged);
                OnHealthChanged(healthBeahvior.Health, healthBeahvior.MaxHealth);
            }
        }
    }

    public void OnHealthChanged(float current, float max)
    {
        bool newActiveValue = current != max;
        if (newActiveValue == this.gameObject.activeSelf)
            return;

        imagesToFade.ForEach(x =>
        {
            if (newActiveValue)
                x.color = new Color(x.color.r, x.color.g, x.color.b, animationTimeSeconds);
            else
                x.color = new Color(x.color.r, x.color.g, x.color.b, animationTimeSeconds);

            x.DOFade(newActiveValue ? 1.0f : 0.0f, 1.0f).OnComplete(() =>
            {
                this.gameObject.SetActive(newActiveValue);
            });

            if (labelToFade != null)
                labelToFade.DOFade(newActiveValue ? 1.0f : 0.0f, animationTimeSeconds);
        });

        /*
         * When we are active need to show immediatly
         */
        if (newActiveValue)
            this.gameObject.SetActive(newActiveValue);
    }
}
