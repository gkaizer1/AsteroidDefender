using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RepairableBehavior : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject repairIcon;

    GameObject _spanner = null;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<HealthBehavior>().OnHealthChanged.AddListener(OnHealthChanged);

        Message.AddListener<GameEventMessage>(OnMessage);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<GameEventMessage>(OnMessage);
        this.GetComponent<HealthBehavior>().OnHealthChanged.RemoveListener(OnHealthChanged);
        if (_spanner != null)
            Destroy(_spanner);
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        if (SelectionManager.SelectedGameObject != this.gameObject)
            return;

        if (message.EventName == "ACTION.REPAIR")
            Repair();
    }

    public void Repair()
    {
        var healthBehavior = this.GetComponent<HealthBehavior>();
        DG.Tweening.DOTween.To(
            () => healthBehavior.Health,
            x => healthBehavior.Health = x,
            healthBehavior.MaxHealth,
            2.0f);
    }

    public void OnHealthChanged(float currentHealth, float maxHealth)
    {
        if (currentHealth == maxHealth)
        {
            if (_spanner != null)
                _spanner.GetComponent<Animator>().SetTrigger("destroy");
            return;
        }

        if (_spanner == null)
        {
            _spanner = Instantiate(repairIcon, this.transform);
        }
    }
}
