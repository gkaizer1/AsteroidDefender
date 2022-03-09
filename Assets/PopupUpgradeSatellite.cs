using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PopupUpgradeSatellite : MonoBehaviour
{
    public List<WeaponSlotUI> WeaponPanels;
    public List<UpgradeWeaponPanelInfo> UpgradePanels;


    void Start()
    {
        var satellite = SelectionManager.SelectedGameObject.GetComponent<SatelliteBehavior>();

        for (int i = 0; i < satellite.WeaponSockets.Count; i++)
        {
            var weaponSocket = satellite.WeaponSockets[i];
            WeaponPanels[i].SetWeapon(weaponSocket);
        }

        // Hide slots that cannot be used
        for (int i = satellite.WeaponSockets.Count; i < WeaponPanels.Count; i++)
        {
            WeaponPanels[i].gameObject.SetActive(false);
        }
    }

    public void OnSlotSelected(WeaponSlot slot)
    {

    }
}
