using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelBehavior : MonoBehaviour
{
    UpgradableBehavior _currentUpgradable;

    [Header("Upgrade Parameters")]
    public UpgradableBehavior.Upgrades upgrade;
    public List<UpgradableBehavior.Upgrades> requirements = new List<UpgradableBehavior.Upgrades>();

    [Header("UIElements")]
    public Button button;
    public TMPro.TextMeshProUGUI indicatorText;
    public List<GameObject> objectsToToggle = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SelectionManager.OnSelectedObjectChanged += SelectionManager_OnSelectedObjectChanged;
    }

    private void SelectionManager_OnSelectedObjectChanged(GameObject previous, GameObject current)
    {
        if (!this.gameObject.activeInHierarchy)
            return;

        if(_currentUpgradable != null)
        {
            _currentUpgradable.OnUpgradeUnlocked.RemoveListener(OnUpgradeUnlocked);
        }

        if (current == null)
        {
            return;
        }

        _currentUpgradable = current.GetComponentInChildren<UpgradableBehavior>();
        if (_currentUpgradable == null)
            return;

        _currentUpgradable.OnUpgradeUnlocked.AddListener(OnUpgradeUnlocked);
        OnUpgradeUnlocked(UpgradableBehavior.Upgrades.UNKNOWN);
    }

    private void OnEnable()
    {
        OnUpgradeUnlocked(UpgradableBehavior.Upgrades.UNKNOWN);
    }

    public void SetHasUpgrade(bool hasUpgrade)
    {
        foreach (var obj in objectsToToggle)
            obj.SetActive(!hasUpgrade);

        if (indicatorText != null)
        {
            if (hasUpgrade)
            {
                indicatorText.text = "Unlocked";
                indicatorText.color = Color.green;
            }
            else
            {
                indicatorText.text = "";
                indicatorText.color = Color.red;
            }
        }

        if (button != null)
        {
            ToggleUpgradeButton(!hasUpgrade);
        }
    }

    public void ToggleUpgradeButton(bool enabled)
    {
        foreach (var obj in button.GetComponents<ResourceBoundButtonBehavior>())
            obj.enabled = enabled;

        button.interactable = enabled;
    }

    public void OnUpgradeUnlocked(UpgradableBehavior.Upgrades upgrade)
    {
        if (_currentUpgradable == null)
            return;

        foreach(var req in requirements)
        {
            if(!_currentUpgradable.activeUpgrades.Contains(req))
            {
                indicatorText.text = "LOCKED";
                indicatorText.color = Color.red;
                ToggleUpgradeButton(false);
                return;
            }
        }

        bool hasUpgrade = _currentUpgradable.activeUpgrades.Contains(this.upgrade);
        SetHasUpgrade(hasUpgrade);
    }
}
