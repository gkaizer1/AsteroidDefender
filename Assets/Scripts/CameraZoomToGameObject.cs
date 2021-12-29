using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraZoomToGameObject : MonoBehaviour
{
    Cinemachine.CinemachineVirtualCamera _camera;
    public float lookTime = 2f;

    Queue<GameObject> _lookQueue = new Queue<GameObject>();
    Coroutine _co_routine = null;
    public UnityEvent OnLookEnded;

    // Start is called before the first frame update
    void Awake()
    {
        _camera = this.GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    public void ZoomToGameObject(GameObject gameObject)
    {
        _lookQueue.Enqueue(gameObject);

        if (lookTime > 0)
        {
            if (_co_routine == null)
                _co_routine = StartCoroutine(co_DisableCamera());
        }
    }

    IEnumerator co_DisableCamera()
    {
        while (_lookQueue.Count > 0)
        {
            var gameObj = _lookQueue.Dequeue();
            _camera.Follow = gameObj.transform;
            _camera.enabled = true;
            yield return new WaitForSeconds(lookTime);
            _camera.enabled = false;
        }
        _co_routine = null;
    }
}
