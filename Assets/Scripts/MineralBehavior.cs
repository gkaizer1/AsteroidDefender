using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MineralBehavior : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject indicatorPrefab;
    public GameObject onAddedAnimation;

    [Header("Children")]
    public GameObject indicatorAnchor;

    public Resource resource;
    public float value;

    private GameObject _obj = null;
    private GameObject indicator = null;

    [Header("Settings")]
    public float lifeTimeSeconds = 2.0f;

    Animator _animator;
    Coroutine _killCoRoutine = null;

    void Start()
    {
        GameEventMessage.AddListener<GameEventMessage>(OnMessage);
        _killCoRoutine = StartCoroutine(KillCoroutine());

        indicator = Instantiate(indicatorPrefab);
        indicator.GetComponent<IndicatorBehavior>().anchor_point = indicatorAnchor.transform;
        indicator.GetComponent<IndicatorBehavior>().AlwaysShow = true;
        indicator.GetComponent<IndicatorBehavior>().onIndicatorClicked.AddListener(OnCollectMineralClicked);

        _animator = this.GetComponent<Animator>();
    }

    public IEnumerator KillCoroutine()
    {
        float yieldSeconds = 0.5f;
        while(this.gameObject != null)
        {
            yield return new WaitForSeconds(yieldSeconds);
            lifeTimeSeconds -= yieldSeconds;
            if(lifeTimeSeconds < 10.0f)
            {
                _animator.SetTrigger("timeout");
                _animator.speed = Mathf.Clamp(10.0f / lifeTimeSeconds, 1f, 3f);
            }
            if(lifeTimeSeconds <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        GameEventMessage.RemoveListener<GameEventMessage>(OnMessage);
        if (indicator != null)
        {
            indicator.GetComponent<IndicatorBehavior>().onIndicatorClicked.RemoveListener(OnCollectMineralClicked);
            Destroy(indicator);
        }
    }

    public void OnCollectMineralClicked()
    {
        ShuttleMission_CollectResoure mission = new ShuttleMission_CollectResoure(this.gameObject);
        ShuttleMissionManager.Instance.ScheduleMission(mission);
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        if (message.EventName.StartsWith("SHUTTLE.COLLECT_MISSION"))
        {
            var customObject = message.CustomObject;
            if(this.gameObject == customObject)
            {
                if (indicator != null)
                    indicator.SetActive(false);

                StopCoroutine(_killCoRoutine);
                _animator.ResetTrigger("timeout");
                _animator.SetTrigger("exit");
            }
        }
    }

    public void AddToResource()
    {
        _obj = Instantiate(onAddedAnimation);
        _obj.gameObject.transform.position = this.transform.position;
        _obj.GetComponentInChildren<TextMeshProUGUI>().text = "+" + this.value;

        ResourceManagerBehavior.Instance.AddResource(this.resource, this.value);
    }
}
