using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LaserBehavior : MonoBehaviour
{
    StateManager<LaserBehavior> _state;
    public string CurrentState => _state?.Name;

    public GameObject laserTopAnchor;
    public LineRenderer lineRenderer;
    public Transform laserTip;

    public float RotationAnglePerSecond = 90.0f;

    public float chargePerSecond = 10.0f;
    public float damagePerSecond = 10.0f;

    public GameObject LaserStart;
    public GameObject LaserEnd;

    public GameObject FiringLight;

    public List<GameObject> powerLights = new List<GameObject>();

    public bool RequiresPowerToFire = true;


    // Start is called before the first frame update
    void Start()
    {
        _state = new StateManager<LaserBehavior>(new LaserIdleState(this));
    }

    // Update is called once per frame
    void Update()
    {
        _state.Update();
    }

    public void UnlockCooler()
    {
        this.GetComponent<AutoAimBehavior>().recalculateAimTime = 0.2f;
        this.GetComponent<AutoAimBehavior>().range *= 1.5f;
    }

    public void UnlockCamera()
    {
        this.GetComponent<AutoAimBehavior>().recalculateAimTime = 0.2f;
        this.GetComponent<AutoAimBehavior>().range *= 1.5f;
    }

    public void UnlockRadar()
    {
        this.GetComponent<AutoAimBehavior>().recalculateAimTime = 0.2f;
        this.GetComponent<AutoAimBehavior>().range *= 1.5f;
    }

    public void UnlockBattery()
    {
        this.GetComponent<AutoAimBehavior>().recalculateAimTime = 0.2f;
        this.GetComponent<AutoAimBehavior>().range *= 1.5f;
    }
}
