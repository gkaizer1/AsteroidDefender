using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelBehavior : MonoBehaviour
{
    UpgradableBehavior _currentUpgradable;

    [Header("Upgrade Parameters")]
    public Upgrades upgrade;
    public List<Upgrades> requirements = new List<Upgrades>();

    [Header("UIElements")]
    public Button button;
    public TMPro.TextMeshProUGUI indicatorText;
    public List<GameObject> objectsToToggle = new List<GameObject>();

    public void SetUpgradeableParent(UpgradableBehavior upgradeable)
    {
        _currentUpgradable = upgradeable;

        if (_currentUpgradable != null)
        {
            _currentUpgradable.OnUpgradeUnlocked.RemoveListener(OnUpgradeUnlocked);
        }

        _currentUpgradable.OnUpgradeUnlocked.AddListener(OnUpgradeUnlocked);
        OnUpgradeUnlocked();
    }

    private void OnEnable()
    {
        OnUpgradeUnlocked();
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
        // Disable resource bound button
        foreach (var obj in button.GetComponents<ResourceBoundButtonBehavior>())
            obj.enabled = enabled;

        button.interactable = enabled;
    }

    public void OnUpgradeUnlocked(Upgrades upgrade = Upgrades.UNKNOWN)
    {
        if (_currentUpgradable == null)
            return;

        foreach(var req in requirements)
        {
            if (_currentUpgradable.activeUpgrades.Contains(req))
                continue;
            
            if (indicatorText != null)
            {
                indicatorText.text = "LOCKED";
                indicatorText.color = Color.red;
            }

            ToggleUpgradeButton(false);
            break;            
        }

        bool hasUpgrade = _currentUpgradable.activeUpgrades.Contains(this.upgrade);
        SetHasUpgrade(hasUpgrade);
    }
}
