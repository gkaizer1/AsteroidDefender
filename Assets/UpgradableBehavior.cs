using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public enum Upgrades
{
    VULCAN_CANNON_2 = 0,
    VULCAN_CANNON_3 = 1,
    VULCAN_AMMO_BOX = 2,
    VULCAN_AMMO = 3,
    VULCAN_RADAR = 4,
    LASER_HEAT_SINK = 5,
    LASER_CAMERA = 6,
    LASER_BATTERY = 7,
    LASER_RADAR = 8,
    MISSLE_POD_2 = 9,
    MISSLE_POD_3 = 10,
    MISSLE_LEVEL_2 = 11,
    MISSLE_LEVEL_3 = 12,
    MISSLE_RADAR = 13,
    UNKNOWN = -1
};

[System.Serializable]
public class UpgradableEvent
{
    public string UIEvent;
    public Upgrades upgrade;
    public UnityEvent callbacks;
    public List<Upgrades> requiredUpgrades;
}

public class UpgradableBehavior : MonoBehaviour
{

    public List<UpgradableEvent> upgrades = new List<UpgradableEvent>();

    public List<Upgrades> activeUpgrades = new List<Upgrades>();

    public UnityEvent<Upgrades> OnUpgradeUnlocked;

    // Start is called before the first frame update
    void Awake()
    {
        GameEventMessage.AddListener<GameEventMessage>(OnMessage);
    }

    public void OnDestroy()
    {
        GameEventMessage.RemoveListener<GameEventMessage>(OnMessage);
    }

    public void UnlockUpgrade(Upgrades upgrade)
    {
        this.activeUpgrades.Add(upgrade);

        upgrades.Where(x => x.upgrade == upgrade).ToList().ForEach(x =>
        {
            x.callbacks?.Invoke();
        });
        OnUpgradeUnlocked?.Invoke(upgrade);
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        // Only run the upgrade if this is the selected game object
        if (!Utils.IsParent(this.gameObject, SelectionManager.SelectedGameObject))
            return;

        string eventName = message.EventName;
        foreach (var handler in upgrades)
        {
            if (handler.UIEvent != eventName)
                continue;

            if (this.activeUpgrades.Contains(handler.upgrade))
                continue;

            foreach (var upgrade in handler.requiredUpgrades)
            {
                if (!this.activeUpgrades.Contains(upgrade))
                {
                    // TODO:: Show required upgrade
                    return;
                }
            }

            this.activeUpgrades.Add(handler.upgrade);

            handler.callbacks?.Invoke();
            OnUpgradeUnlocked?.Invoke(handler.upgrade);
        }
    }
}
