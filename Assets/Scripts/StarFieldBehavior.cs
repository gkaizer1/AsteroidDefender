using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldBehavior : MonoBehaviour
{
    Camera _mainCamera;

    ParticleSystem _particleSystem = null;

    Vector3 _previousCameraPosition;

    public float maxStars = 100.0f;
    public float lifeTime = 20f;


    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        _previousCameraPosition = _mainCamera.transform.position;
        _particleSystem = GetComponent<ParticleSystem>();

        _particleSystem.transform.localPosition = new Vector3((Screen.width / 2.0f), _particleSystem.transform.position.y, _particleSystem.transform.position.z);

        EventManager.OnCameraOrthograficSizeChange += EventManager_OnCameraOrthograficSizeChange;

        UpdateParticleSystem(true);
    }

    private void OnDestroy()
    {
        EventManager.OnCameraOrthograficSizeChange -= EventManager_OnCameraOrthograficSizeChange;
    }

    private void EventManager_OnCameraOrthograficSizeChange(float cameraOrthoSize)
    {
        _particleSystem.transform.localScale = new Vector3(_particleSystem.transform.localScale.x, _particleSystem.transform.localScale.y, _mainCamera.orthographicSize * 2.0f);
        //UpdateParticleSystem();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_previousCameraPosition == _mainCamera.transform.position)
            return;

        //var force = particleSystem.forceOverLifetime;
        //force.z = -(_mainCamera.transform.position.y - _previousCameraPosition.y);
        //_previousCameraPosition = _mainCamera.transform.position;


        //UpdateParticleSystem(false);
    }

    void UpdateParticleSystem(bool reSimulate = false)
    {
        var main = _particleSystem.main;

        main.startLifetime = lifeTime;
        float worldWidth = (_mainCamera.orthographicSize * Screen.width / Screen.height) * 2.0f;
        main.startSpeed = worldWidth / lifeTime;

       var emissions = _particleSystem.emission;
       emissions.rateOverTime = maxStars / lifeTime;

        //var shape = particleSystem.shape;
        //var scale = shape.scale;
        //scale.x = _mainCamera.orthographicSize * 2.0f;
        //shape.scale = scale;

        //float x = _mainCamera.transform.position.x + (_mainCamera.orthographicSize * Screen.width / Screen.height);
        //float y = _mainCamera.transform.position.y;
        //transform.position = new Vector3(x, y, 1.0f);

        if (reSimulate)
        {
            _particleSystem.Simulate(100.0f);
            _particleSystem.Play();
        }
    }
}
