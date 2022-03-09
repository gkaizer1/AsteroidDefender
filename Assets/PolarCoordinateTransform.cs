using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class PolarCoordinateTransform : MonoBehaviour
{
#if UNITY_EDITOR
    float _previousAngle;
    float _previousRadius;
    Vector3 _previousCoordinate;
#endif

    [Range(0, 120)]
    [SerializeField]
    private float _Radius = 0f;
    public float Radius
    {
        set
        {
            _Radius = value;
            UpdatePosition();
        }
        get
        {
            return _Radius;
        }
    }

    [Range(-360, 360)]
    [SerializeField]
    private float _Angle = 0f;
    public float Angle
    {
        set
        {
            _Angle = value;

            // NOTE:: Polar coordinate should NOT rotate the object, only MOVE it
            // Rotate object to match the current relative rotation
            //this.transform.rotation = Quaternion.Euler(0f, 0f, value);

            UpdatePosition();
        }
        get
        {
            return _Angle;
        }
    }

    public void UpdatePosition()
    {
        this.transform.position = Utils.PolarToCartesian(Angle, Radius);

#if UNITY_EDITOR
        _previousAngle = Angle;
        _previousRadius = Radius;
        _previousCoordinate = this.transform.position;
#endif
    }

#if UNITY_EDITOR
    void Update()
    {
        if ((this.transform.position - _previousCoordinate).magnitude > 0.1f)
        {
            var coordinates = Utils.CartesianToPolar(this.transform.position);
            _Angle = coordinates.angle;
            _Radius = coordinates.radius;
            UpdatePosition();
            return;
        }

        if (this._Radius != _previousRadius || this._Angle != _previousAngle)
        {
            _previousAngle = _Angle;
            _previousRadius = _Radius;
            UpdatePosition();
            return;
        }
    }
#endif
}
