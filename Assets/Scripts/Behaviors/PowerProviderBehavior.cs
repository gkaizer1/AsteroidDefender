using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PowerProviderBehavior : MonoBehaviour
{
    [Header("Power Settings")]
    public float MWPerSecond = 1.0f;
    public float SecondsBetweenPowerGen = 1.0f;
    public float powerRange = 15.0f;

    [Header("Events")]
    public PowerChangedEvent OnPowerChanged;
    public PowerChangedEvent OnStoredChargeChanged;
    public UnityEvent OnPowerConsumersChanged;

    [Header("Children")]
    public GameObject powerRangeComonent;

    [HideInInspector]
    public List<PowerConsumer> powerConsumers = new List<PowerConsumer>();
    [HideInInspector]
    public Animator _animator;

    public float maxStoredCharge = 100.0f;
    public float currentStoredCharge = 0.0f;

    void Start()
    {
        _animator = GetComponent<Animator>();

        StartCoroutine(PowerGenaratorCoRoutine());
        StartCoroutine(UpdatePowerConsumers());
    }

    IEnumerator UpdatePowerConsumers()
    {
        var wait = new WaitForSeconds(2.0f);
        int tiles_layer = LayerMask.NameToLayer("Tiles");

        while (this.gameObject != null)
        {
            lock (this.gameObject)
            {
                // Cleanup out of range or null power consumers
                powerConsumers.RemoveAll(x => x == null);
                powerConsumers.RemoveAll(x => (x.transform.position - this.transform.position).magnitude > powerRange);

                // Find all Power consumers in range
                bool consumersChanged = false;
                var itemsInRadius = Physics2D.OverlapCircleAll(this.transform.position, powerRange, (1 << tiles_layer));
                foreach (Collider2D collider in itemsInRadius)
                {
                    var consumer = collider.gameObject.GetComponentInChildren<PowerConsumer>();
                    if (consumer != null && !powerConsumers.Contains(consumer))
                    {
                        consumersChanged = true;
                        powerConsumers.Add(consumer);
                    }
                }
                if (consumersChanged)
                    OnPowerConsumersChanged.Invoke();
            }
            yield return wait;
        }
    }

    IEnumerator PowerGenaratorCoRoutine()
    {
        var wait = new WaitForSeconds(SecondsBetweenPowerGen);
        while (this.gameObject != null)
        {
            lock (this.gameObject)
            {
                float generatedPower = MWPerSecond * SecondsBetweenPowerGen;

                float currentPower = generatedPower;

                // Pass #1 give everyone their fair share of power (for 5 passes)
                float count = 5;
                while (currentPower > 0.0f && (count--) > 0)
                {                    
                    float powerPerConsumer = currentPower / (float)powerConsumers.Where(x => x.CurrentPower != x.MaxPower).Count();
                    foreach (var x in powerConsumers)
                    {
                        if (x != null && currentPower > 0.0f)
                            currentPower = currentPower - x.ProvidePower(powerPerConsumer);
                    }
                }

                // Pass #2 feed the items that need more power
                if (currentPower > 0.1f)
                {
                    foreach (var x in powerConsumers)
                    {
                        if (x != null && currentPower > 0.0f)
                            currentPower = currentPower - x.ProvidePower(currentPower);
                    }
                }

                if (_animator != null)
                {
                    if (currentPower > 0.1f)
                        _animator.StopPlayback();
                    else
                        _animator.StartPlayback();
                    _animator.SetBool("OUT_OF_POWER", currentPower == 0);
                }

                if(currentStoredCharge < maxStoredCharge && currentPower > 0.1f)
                {
                    currentStoredCharge = Mathf.Clamp(currentStoredCharge + currentPower, 0f, maxStoredCharge);
                    OnStoredChargeChanged.Invoke(currentStoredCharge, maxStoredCharge);
                }
                else if(currentStoredCharge > 0f)
                {
                    float powerPerConsumer = currentStoredCharge / (float)powerConsumers.Where(x => x.CurrentPower != x.MaxPower).Count();
                    foreach (var x in powerConsumers)
                    {
                        if (x != null && currentPower > 0.0f)
                            currentStoredCharge = currentStoredCharge - x.ProvidePower(powerPerConsumer);
                    }
                    OnStoredChargeChanged.Invoke(currentStoredCharge, maxStoredCharge);
                }

                // Emit how much power we have vs how much we generated
                OnPowerChanged?.Invoke(currentPower, generatedPower);
            }
            yield return wait;
        }
    }

    public void RegisterPowerConsumer(PowerConsumer consumer)
    {
        powerConsumers.Add(consumer);
    }

    public void OnSelectionChanged(GameObject previous, GameObject current)
    {
        var audioSource = GetComponent<AudioSource>();
        bool isSelected = this.gameObject.IsParent(current);
        if (powerRangeComonent != null)
        {
            powerRangeComonent.SetActive(isSelected);
            powerRangeComonent.GetComponent<CircleBehavior>().radius = powerRange;
        }

        if (isSelected)
        {
            GetComponent<PowerSelectedBehavior>().enabled = true;
            audioSource.enabled = true;
            audioSource.Play();
            audioSource.volume = Utils.GetAudioVolumeFromCamera();
            audioSource.DOFade(0.0f, 4.0f).OnComplete(() => audioSource.enabled = false);
        }
        else
        {
            audioSource.enabled = false;
            GetComponent<PowerSelectedBehavior>().enabled = false;
        }

    }
}
