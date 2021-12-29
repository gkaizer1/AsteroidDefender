using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CameraTransitionInformation
{
    public Cinemachine.CinemachineVirtualCamera targetCam;
    public float transtionTime;
    public UnityEvent OnCameraEnabled;
    public UnityEvent onTimer;
    public bool Continue = true;
}

public class TransitionCamera : MonoBehaviour
{
    ICinemachineCamera _currentCam;
    public List<CameraTransitionInformation> transitions = new List<CameraTransitionInformation>();
    public bool playOnWake = false;

    CameraTransitionInformation _waitingTransition = null;
    bool cancel = false;

    private void Awake()
    {
        if (playOnWake)
            StartTransitions();
    }

    public void Cancel()
    {
        cancel = true;
    }

    public void StartTransitions()
    {
        _currentCam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        StartCoroutine(TransitionBack());
    }

    public void RevertCamera()
    {
        _currentCam.VirtualCameraGameObject.SetActive(true);
    }

    public void Continue()
    {
        if(_waitingTransition != null)
            _waitingTransition.Continue = true;
    }

    public IEnumerator TransitionBack()
    {
        foreach(var transition in transitions)
        {
            if (cancel)
                break;

            Cinemachine.CinemachineVirtualCamera targetCam = transition.targetCam;
            targetCam.enabled = true;
            ZoomBehavior.Instance.LockCamera();
            try
            {
                transition.OnCameraEnabled?.Invoke();
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }

            yield return new WaitForSeconds(transition.transtionTime);


            try
            {
                transition.onTimer?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            ZoomBehavior.Instance.UnlockCamera();
            while (!transition.Continue)
            {
                _waitingTransition = transition;
                yield return new WaitForSeconds(0.1f);
            }

            targetCam.enabled = false;
            _waitingTransition = null;
        }
    }
}
