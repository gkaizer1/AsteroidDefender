using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleStatusBehavior : MonoBehaviour
{
    private GameObject _uiObject;
    // Start is called before the first frame update
    void Start()
    {
        var canvas = GameObject.FindGameObjectsWithTag("text_canvas")[0] as GameObject;
        _uiObject = Instantiate(Resources.Load("CIRCLE_STATUS_PREFAB"), canvas.transform) as GameObject;
        _uiObject.transform.localScale = Vector3.one * 0.25f;
        _uiObject.name = $"circle_status_{this.name}";
    }

    // Update is called once per frame
    void Update()
    {
        _uiObject.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
    }

    public void SetFill(float fill)
    {
        if (_uiObject == null)
            return;

        _uiObject.GetComponentInChildren<CircleFillerBehavior>().SetFill(fill);
    }

    private void OnDestroy()
    {
        Destroy(this._uiObject);
    }
}
