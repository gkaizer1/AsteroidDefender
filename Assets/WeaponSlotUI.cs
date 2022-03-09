using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image WeaponIcon;
    public WeaponSlot weaponSlot;

    public void SetWeapon(WeaponSlot weaponSlot)
    {
        var equipedWeapon = weaponSlot.equipedWeapon;
        if (equipedWeapon == null)
            WeaponIcon.enabled = false;
        else
        {
            WeaponIcon.enabled = true;
            WeaponIcon.sprite = equipedWeapon.WeaponImage;
        }

        weaponSlot = weaponSlot;
    }
}
