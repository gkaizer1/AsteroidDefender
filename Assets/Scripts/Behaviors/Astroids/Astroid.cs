using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum AsteroidType
{
    SMALL,
    FIRE,
    ICE,
    METAL,
    METOERITE
}

public class Astroid : MonoBehaviour, IStateMachine
{
    public AsteroidType asteroidType = AsteroidType.SMALL;

    public event Action<GameObject> OnCollision;

    [Header("State")]
    public StateManager<Astroid> _stateManager;

    [Header("Children")]
    public GameObject mainSprite;
    public LineRenderer lineRenderer;
    public Rigidbody2D rigidBody;
    public GameObject fireEffect;

    [Header("Settings")]
    public AstroidSettings settings;
    public List<string> effectsToEnableOnActivated = new List<string>();

    [Header("Prefabs")]
    public GameObject targetIndicatorPrefab;
    public GameObject damageParticleEffect;
    public GameObject impactExplosion;

    [HideInInspector]
    public GameObject _targetIndicatorInstance;
    public Vector3 _targetPoint;

    float earthRadius = 0.0f;
    public float _earthCollisionRadius = 0.0f;
    public bool IsInsideEarth = false;
    public bool IsOnCourse = false;

    public enum InitialBehavior { Orbit, FlyToEarth };
    public InitialBehavior initialBehavior = InitialBehavior.FlyToEarth;
    public float initialVelocity = 0.0f;

    public IndicatorBehavior _indicator;

    Tween _rescaleTween = null;

    private float _scale = 1.0f;
    private float _initialScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _indicator = this.GetComponent<IndicatorBehavior>();
        if (_indicator != null)
            _indicator.enabled = false;

        earthRadius = GameObject.FindGameObjectWithTag("earth").GetComponent<CircleCollider2D>().radius;
        _earthCollisionRadius = earthRadius * 1f;

        this.name = $"Astroid-{Guid.NewGuid().ToString().ToUpper().Substring(0, 6)}";

        // Update from settings
        if (settings != null)
        {
            float scale = UnityEngine.Random.Range(settings.MinScale, settings.MaxScale);
            transform.localScale = Vector3.one * scale;
            rigidBody.mass = rigidBody.mass * scale;
            _scale = scale;
            _initialScale = _scale;
        }

        // Aim at a random point on the earth
        _targetPoint = Utils.GenerateRandomPointInEarth();

        IState<Astroid> state = null;

        // Set intial state
        switch (initialBehavior)
        {
            case InitialBehavior.Orbit:
                state = new AstroidOrbitingState(this);
                break;
            case InitialBehavior.FlyToEarth:
                state = new AstroidFlyingState(this, initialVelocity);
                break;
            default:
                state = new AstroidOrbitingState(this);
                break;
        }

        var initialState = new EnemySpawningBehavior<Astroid>(this).OnSpawnCompleted(() =>
        {
            return new AstroidSpawningState(this, state);
        });
        _stateManager = new StateManager<Astroid>(initialState);

        EventManager.OnCameraOrthograficSizeChange += OnCameraOrthograficSizeChange;
        OnCameraOrthograficSizeChange(Camera.main.orthographicSize);

        AstroidManager.Astroids.Add(this.gameObject);

        OnCollision += Astroid_OnCollision;
    }
    private void Astroid_OnCollision(GameObject obj)
    {
        if(obj.CompareTag("astroid"))
        {
            float health = GetComponent<HealthBehavior>().Health;
            if(health >= obj.GetComponent<HealthBehavior>().Health)
            {
                float otherAsteroidHealth = obj.GetComponent<HealthBehavior>().Health / 2.0f;

                float increasePrecentage = 1.0f + otherAsteroidHealth / GetComponent<HealthBehavior>().MaxHealth;

                obj.GetComponent<HealthBehavior>().Health = 0;
                GetComponent<HealthBehavior>().MaxHealth += otherAsteroidHealth;
                GetComponent<HealthBehavior>().Health += otherAsteroidHealth;

                _scale = Mathf.Clamp(_scale * increasePrecentage, 0f, _initialScale * 2.0f);
                if (_rescaleTween != null)
                    _rescaleTween.Complete();
                _rescaleTween = transform.DOScale(Vector3.one * _scale, 0.5f).OnComplete(() => _rescaleTween = null);
            }
        }
    }

    private void OnCameraOrthograficSizeChange(float cameraOrthoSize)
    {
        var rotator = GetComponentInChildren<RotatorBehavior>();
        if (rotator != null)
            rotator.enabled = cameraOrthoSize < 30.0f;
    }

    public void OnDestroy()
    {
        _stateManager.OnDestroy();
        EventManager.OnCameraOrthograficSizeChange -= OnCameraOrthograficSizeChange;
        if (_targetIndicatorInstance != null)
        {
            Destroy(_targetIndicatorInstance);
            _targetIndicatorInstance = null;
        }
        if (_rescaleTween != null)
        {
            _rescaleTween.Kill();
            _rescaleTween = null;
        }
        AstroidManager.Astroids.Remove(this.gameObject);
    }

    public GameObject CreateTargetIndicator()
    {
        if (targetIndicatorPrefab == null)
            return null;

        // Lazy initialize the target indicator
        var targetIndicatorInstance = Instantiate(targetIndicatorPrefab, AstroidsContainer.Instance.transform);
        targetIndicatorInstance.name = $"{name}_indicator";
        targetIndicatorInstance.transform.position = new Vector3(_targetPoint.x, _targetPoint.y, 1.0f);
        return targetIndicatorInstance;
    }

    // Update is called once per frame
    void Update()
    {
        _stateManager.Update();
    }

    private void FixedUpdate()
    {
        _stateManager.FixedUpdate();
    }

    public string CurrentState => _stateManager.Name;

    public void OnSelectionChanged(GameObject previous, GameObject current)
    {
        if (current == this.gameObject)
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, this.transform.position);
                lineRenderer.SetPosition(1, new Vector3(_targetPoint.x, _targetPoint.y, 1.0f));
            }
            if (_targetIndicatorInstance == null)
                _targetIndicatorInstance = CreateTargetIndicator();

            if (_targetIndicatorInstance == null)
                _targetIndicatorInstance.SetActive(true);
        }
        if (previous == this.gameObject)
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;

            if (_targetIndicatorInstance != null)
                _targetIndicatorInstance.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollision?.Invoke(collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision?.Invoke(collision.gameObject);
    }

    public void OnDetected()
    {
        if(mainSprite != null)
            mainSprite.SetActive(true);
    }

    public void OnHidden()
    {
        if (mainSprite != null)
            mainSprite.SetActive(false);
    }
}
