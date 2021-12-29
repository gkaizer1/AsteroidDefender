using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCenterBehavior : MonoBehaviour
{
    public Texture2D selectTargetCursor;
    [Header("Prefab")]
    public GameObject superNukePrefab;
    public GameObject superNukeTargetIndicator;

    bool _selectingSuperNukeTarget = false;

    public float SecondsToSuperNuke = 60.0f;
    private float _secondsLeftToSuperNuke = 0.0f;
    public static event Action<float> OnTimeToSuperNukeChanged;

    // Start is called before the first frame update
    void Start()
    {
        _secondsLeftToSuperNuke = SecondsToSuperNuke;

        Message.AddListener<GameEventMessage>(OnMessage);

        GameEventMessage.SendEvent("HIDE_SUPER_WEAPONS_PANEL");
        GameEventMessage.SendEvent("HIDE_LAUNCH_SUPER_WEAPON_PANEL");
        GameEventMessage.SendEvent("SHOW_SUPER_NUKE_COUNTER");
    }

    private void OnDestroy()
    {
        Message.RemoveListener<GameEventMessage>(OnMessage);
    }

    private void FixedUpdate()
    {
        if (_secondsLeftToSuperNuke == 0)
            return;

        _secondsLeftToSuperNuke = Mathf.Clamp(_secondsLeftToSuperNuke - Time.deltaTime, 0, float.MaxValue);
        OnTimeToSuperNukeChanged?.Invoke(_secondsLeftToSuperNuke);
        if (_secondsLeftToSuperNuke == 0)
        {
            GameEventMessage.SendEvent("SHOW_SUPER_WEAPONS_PANEL", this.gameObject);
            GameEventMessage.SendEvent("HIDE_SUPER_NUKE_COUNTER", this.gameObject);

            var popup = UIPopup.GetPopup("SuperNukeReader");
            UIPopupManager.AddToQueue(popup);
        }
    }

    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        if (message.EventName.StartsWith("LAUNCH.SUPER_NUKE"))
        {
            Cursor.SetCursor(selectTargetCursor, new Vector2(selectTargetCursor.width/2, selectTargetCursor.height/2), CursorMode.Auto);
            GameEventMessage.SendEvent("HIDE_SUPER_WEAPONS_PANEL");
            GameEventMessage.SendEvent("SHOW_LAUNCH_SUPER_WEAPON_PANEL");
            _selectingSuperNukeTarget = true;
        }
        if (message.EventName == "LAUNCH.CANCEL")
        {
            CancelLaunchSuperNuke();
        }
    }

    public void CancelLaunchSuperNuke()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GameEventMessage.SendEvent("SHOW_SUPER_WEAPONS_PANEL");
        GameEventMessage.SendEvent("HIDE_LAUNCH_SUPER_WEAPON_PANEL");
        _selectingSuperNukeTarget = false;
    }

    private void Update()
    {
        if (_selectingSuperNukeTarget)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                LaunchSuperNuke(mousePos);
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CancelLaunchSuperNuke();
            }
        }
    }

    public void LaunchSuperNuke(Vector3 tagetPoint)
    {
        var indicator = Instantiate(superNukeTargetIndicator);
        indicator.transform.position = tagetPoint;

        var nuke = Instantiate(superNukePrefab);
        nuke.transform.position = this.transform.position;
        nuke.transform.rotation = this.transform.rotation;
        nuke.GetComponent<AutoAimBehavior>().currentTarget = indicator;
        var nukeBehavior = nuke.GetComponent<NukeBehavior>();
        nukeBehavior.OnObjectDestroyed += () => Destroy(indicator.gameObject);
        nukeBehavior.Fire();
        nukeBehavior.OnExplosion(explosionPoint =>
        {
            var enemies = Utils.GetEnemiesInCircle(explosionPoint, 4.0f);
            foreach(var enemy in enemies)
            {
                var  health = enemy.GetComponent<HealthBehavior>();
                if (health == null)
                    continue;
                health.Health -= 1000.0f;
            }
        });

        CancelLaunchSuperNuke();
        
        var popup = UIPopup.GetPopup("LaunchSuperNuke");
        UIPopupManager.AddToQueue(popup);

        GameEventMessage.SendEvent("HIDE_SUPER_WEAPONS_PANEL");
        GameEventMessage.SendEvent("HIDE_LAUNCH_SUPER_WEAPON_PANEL");
        GameEventMessage.SendEvent("SHOW_SUPER_NUKE_COUNTER");

        _secondsLeftToSuperNuke = SecondsToSuperNuke;
    }
}
