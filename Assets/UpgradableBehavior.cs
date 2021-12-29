using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UpgradableEvent
{
    public string UIEvent;
    public UpgradableBehavior.Upgrades upgrade;
    public UnityEvent callbacks;
    public List<UpgradableBehavior.Upgrades> requiredUpgrades;
}

public class UpgradableBehavior : MonoBehaviour
{
    public enum Upgrades
    {
        VULCAN_CANNON_2,
        VULCAN_CANNON_3,
        VULCAN_AMMO_BOX,
        VULCAN_AMMO,
        VULCAN_RADAR,
        LASER_HEAT_SINK,
        LASER_CAMERA,
        LASER_BATTERY,
        LASER_RADAR,
        MISSLE_POD_2,
        MISSLE_POD_3,
        MISSLE_LEVEL_2,
        MISSLE_LEVEL_3,
        MISSLE_RADAR,
        UNKNOWN
    };

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
