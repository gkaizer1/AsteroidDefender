using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Doozy.Engine;

public class ZoomBehavior : MonoBehaviour
{
    public static float SENSITIVITY_MULTIPLIER = 2.0f;
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public Cinemachine.CinemachineVirtualCamera zoomCamera;

    public float MIN_WORLD_ORTHO_SIZE = 10.0f;
    public float MAX_WORLD_ORTHO_SIZE = 50.0f;

    [Range(0.0f, 100.0f)]
    public float ZoomFactor = 3.5f;

    Tweener _tween;
    float _targetOrthograficSize;

    public Collider2D boundary;

    [HideInInspector]
    public Vector2 _previousTouchPosition1 = Vector3.zero;
    [HideInInspector]
    public Vector2 _previousTouchPosition2 = Vector3.zero;
    [HideInInspector]
    float _previousTouchDistance = 0.0f;

    bool _cameraLocked = false;
    bool _panning = false;
    Vector3 _lastMousePos = Vector3.zero;

    public static ZoomBehavior Instance = null;

    public static event System.Action OnPanStarted;
    public static event System.Action OnPanEnded;

    private void Awake()
    {
        OnPanEnded += OnPanEndedEvent;
        OnPanStarted += OnPanStartedEvent;
        Instance = this;
    }

    private void OnPanStartedEvent()
    {
        Cursor.visible = false;
    }

    private void OnPanEndedEvent()
    {
        Cursor.visible = true;
    }

    public void LockCamera()
    {
        _cameraLocked = true;
    }

    public void UnlockCamera()
    {
        _cameraLocked = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.InvokeOnWorldSizeChanged();

        Message.AddListener<GameEventMessage>(OnMessage);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<GameEventMessage>(OnMessage);
    }
    private void OnMessage(GameEventMessage message)
    {
        if (message == null)
            return;

        if (message.EventName.StartsWith("LOCK_CAMERA"))
            _cameraLocked = true;
        if (message.EventName.StartsWith("UNLOCK_CAMERA"))
            _cameraLocked = false;
    }

    public void ZoomToPoint(Vector3 point, float orthoSize)
    {
        UpdateZoom(orthoSize - virtualCamera.m_Lens.OrthographicSize);
        _cameraLocked = true;
        virtualCamera.transform.DOMove(point, 1.0f).OnComplete(() => _cameraLocked = false);
    }

    public void ZoomToEarth()
    {
        UpdateZoom(20.0f - virtualCamera.m_Lens.OrthographicSize);
        _cameraLocked = true;
        virtualCamera.transform.DOMove(Vector3.zero, 1.0f).OnComplete(() => _cameraLocked = false);
    }

    void BeginPan()
    {
        _panning = true;
        _lastMousePos = Input.mousePosition;
        OnPanStarted?.Invoke();
    }
    void EndPan()
    {
        _panning = false;
        _lastMousePos = Vector3.zero;
        OnPanEnded?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            UpdateZoom(Input.mouseScrollDelta.y * ZoomFactor);
        }

        if (Input.GetMouseButtonDown(0) && !_panning && !_cameraLocked)
        {
            if(!Utils.IsOverUIElement())
                BeginPan();
        }

        if (Input.GetMouseButtonUp(0) && _panning)
        {
            EndPan();
        }

        if (Input.GetMouseButton(0) && _panning && !_cameraLocked)
        {
            var delta = Input.mousePosition - _lastMousePos;
            _lastMousePos = Input.mousePosition;

            float aspectRatio = (Screen.width / Screen.height);
            float hadjust = SENSITIVITY_MULTIPLIER * (delta.x / Screen.width) * (virtualCamera.m_Lens.OrthographicSize * 2.0f * aspectRatio);
            float vadjust = SENSITIVITY_MULTIPLIER * (delta.y / Screen.height) * (virtualCamera.m_Lens.OrthographicSize * 2.0f);

            Vector3 move = new Vector3(hadjust * -1, vadjust * -1, 0.0f);

            if (move.magnitude > float.Epsilon)
            {
                Vector3 currentPosition = virtualCamera.transform.position;
                float x = Mathf.Clamp(currentPosition.x + move.x, -boundary.bounds.size.x, boundary.bounds.size.x);
                float y = Mathf.Clamp(currentPosition.y + move.y, -boundary.bounds.size.y, boundary.bounds.size.y);
                virtualCamera.transform.position = new Vector3(x, y, currentPosition.z);
                zoomCamera.enabled = false;
            }
        }
        if (Input.touchCount == 0)
        {
            if(_previousTouchPosition1 != Vector2.zero)
                EndPan();
        }
        else
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touch1Pos = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                _previousTouchPosition1 = touch1Pos;
                if (!Utils.IsOverUIElement())
                    BeginPan();
            }

            if (Input.touchCount == 1)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    if (!_cameraLocked)
                    {

                        var delta = _previousTouchPosition1 - touch1Pos;
                        _previousTouchPosition1 = touch1Pos;

                        float aspectRatio = (Screen.width / Screen.height);
                        float hadjust = SENSITIVITY_MULTIPLIER * (delta.x / Screen.width) * (virtualCamera.m_Lens.OrthographicSize * 2.0f * aspectRatio);
                        float vadjust = SENSITIVITY_MULTIPLIER * (delta.y / Screen.height) * (virtualCamera.m_Lens.OrthographicSize * 2.0f);
                        Vector3 move = new Vector3(hadjust, vadjust, 0.0f);
                        virtualCamera.transform.Translate(move);
                        zoomCamera.enabled = false;
                    }
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch touch2 = Input.GetTouch(1);
                Vector2 touch2Pos = touch2.position;
                if (touch2.phase == TouchPhase.Began)
                {
                    _previousTouchPosition2 = touch2Pos;
                    _previousTouchDistance = (touch1Pos - touch2Pos).magnitude;
                }
                if (touch.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    float distance = (touch1Pos - touch2Pos).magnitude;
                    float distanceDelta = distance - _previousTouchDistance;
                    UpdateZoom((distanceDelta / 30.0f) * ZoomFactor);
                    _previousTouchDistance = distance;
                    _previousTouchPosition2 = touch2Pos;
                    _previousTouchPosition1 = touch1Pos;

                }
            }
        }
    }

    public void UpdateZoom(float delta)
    {
        zoomCamera.enabled = false;
        if (_tween != null && _tween.IsPlaying())
        {
            _targetOrthograficSize = _targetOrthograficSize - delta;
            _tween.Kill();
        }
        else
        {
            _targetOrthograficSize = virtualCamera.m_Lens.OrthographicSize - delta;
        }

        if (_targetOrthograficSize < 1.0)
            _targetOrthograficSize = 1.0f;

        if (_targetOrthograficSize < MIN_WORLD_ORTHO_SIZE)
        {
            _targetOrthograficSize = MIN_WORLD_ORTHO_SIZE;
        }
        else if (_targetOrthograficSize > MAX_WORLD_ORTHO_SIZE)
        {
            _targetOrthograficSize = MAX_WORLD_ORTHO_SIZE;
        }

        _tween = DG.Tweening.DOTween.To(
            () => virtualCamera.m_Lens.OrthographicSize,
            x => virtualCamera.m_Lens.OrthographicSize = x,
            _targetOrthograficSize,
            0.1f).OnComplete(() => _tween = null).OnKill(() => _tween = null);
        EventManager.InvokeOnCameraOrthograficSizeChanged(virtualCamera.m_Lens.OrthographicSize);

    }
}
