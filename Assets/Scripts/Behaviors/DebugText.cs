using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    List<(PropertyInfo, object)> _properties = new List<(PropertyInfo, object)>();
    List<(FieldInfo, object)> _fields = new List<(FieldInfo, object)>();

    DateTime _lastUpdate = DateTime.MinValue;
    Vector3 ParentBounds;
    public GameObject Parent = null;

    void UpdateProperties()
    {
        if (_properties.Count == 0 && _fields.Count == 0)
        {
            foreach (var monoBehavior in Parent.GetComponents<MonoBehaviour>())
            {
                foreach (var property in monoBehavior.GetType().GetProperties())
                {
                    if (property.GetCustomAttributes(typeof(DebugPropertyAttribute), false).Length > 0)
                    {
                        _properties.Add((property, monoBehavior));
                    }
                }
                foreach (var field in monoBehavior.GetType().GetFields())
                {
                    if (field.GetCustomAttributes(typeof(DebugPropertyAttribute), false).Length > 0)
                    {
                        _fields.Add((field, monoBehavior));
                    }
                }
            }
            _lastUpdate = DateTime.Now;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Parent == null)
        {
            Destroy(this.gameObject);
            return;
        }

        if ((DateTime.Now - _lastUpdate).TotalSeconds > 0.5)
        {
            var rotation = this.Parent.transform.rotation;
            this.Parent.transform.rotation = new Quaternion(0, 0, 0, 0);
            var renderer = Parent.GetComponent<Renderer>();
            if (renderer != null)
            {
                ParentBounds = renderer.bounds.size;
                this.Parent.transform.rotation = rotation;
            }

            UpdateProperties();
        }
        var ParentCenter = Parent.GetComponent<Renderer>().bounds.center;

        // Final position of marker above GO in world space
        Vector3 offsetPos = new Vector3(ParentCenter.x + ParentBounds.x * Parent.transform.localScale.x, ParentCenter.y, ParentCenter.z);

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
        var text = this.GetComponent<Text>();
        text.transform.position = screenPoint;
        List<string> str = new List<string>();
        _properties.ForEach(x =>
        {
            try
            {
                str.Add($"{x.Item1.Name}={x.Item1.GetValue(x.Item2).ToString()}");
            }
            catch
            {
            }
        });
        _fields.ForEach(x =>
        {
            try
            {
                str.Add($"{x.Item1.Name}={x.Item1.GetValue(x.Item2).ToString()}");
            }
            catch
            {
            }
        });
        text.text = string.Join("\n", str);

    }
}
