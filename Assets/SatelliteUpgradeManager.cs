using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class UpgradeWeaponPanelInfo
{
    public GameObject panel;
    public Upgrades Upgrade;
}

public class SatelliteUpgradeManager : MonoBehaviour
{
    public GameObject BuildNewWeaponPanel;
    public GameObject UpgradeWeaponPanel;
    public int CurrentSlotIndex = -1;
    public UnityEvent OnBuildEvent;

    public List<UpgradeWeaponPanelInfo> UpgradePanels = new List<UpgradeWeaponPanelInfo>();

    public void SetCurrentSlot(int index)
    {
        CurrentSlotIndex = index;

        var satellite = SelectionManager.SelectedGameObject.GetComponent<SatelliteBehavior>();
        var weaponSocket = satellite.WeaponSockets[CurrentSlotIndex];

        BuildNewWeaponPanel.SetActive(weaponSocket.equipedWeapon == null);
        UpgradeWeaponPanel.SetActive(weaponSocket.equipedWeapon != null);

        foreach (var upgradePanel in UpgradePanels)
        {
            upgradePanel.panel.SetActive(false);
        }

        if(weaponSocket.equipedWeapon != null)
        {
            /*
             * enable all upgrade that apply to this upgrdable weapon
             */
            UpgradableBehavior upgradeBehavior = weaponSocket.equipedWeapon.GetComponent<UpgradableBehavior>();
            foreach(var weaponUpgrade in upgradeBehavior.upgrades)
            {
                UpgradePanels
                    .Where(x => x.Upgrade == weaponUpgrade.upgrade)
                    .ToList()
                    .ForEach(x => {
                        x.panel.SetActive(true);

                        var upgradeablePanel = x.panel.GetComponent<UpgradePanelBehavior>();
                        upgradeablePanel?.SetUpgradeableParent(upgradeBehavior);
                    });
            }
        }

        Debug.Log($"Setting CurrentSlotIndex={CurrentSlotIndex}");
    }

    public void AddWeapon(GameObject prefab)
    {
        var satellite = SelectionManager.SelectedGameObject.GetComponent<SatelliteBehavior>();
        satellite.WeaponSockets[CurrentSlotIndex].AddWeapon(prefab);

        OnBuildEvent?.Invoke();
    }

    public void UpgradeCurrentSlot(int upgradeEnum)
    {
        var satellite = SelectionManager.SelectedGameObject.GetComponent<SatelliteBehavior>();
        UpgradableBehavior upgradeableBehavior = satellite.WeaponSockets[CurrentSlotIndex].equipedWeapon.GetComponent<UpgradableBehavior>();
        upgradeableBehavior.UnlockUpgrade((Upgrades)(upgradeEnum));

    }
}
