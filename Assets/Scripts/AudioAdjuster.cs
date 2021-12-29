using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAdjuster : MonoBehaviour
{
    AudioSource _audioSource;
    [Range(0f, 1f)]
    public float volumeMultiplier = 1.0f;

    public float fadeTime = 0.0f;
    public float maxOrthoDistance = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = volumeMultiplier * Utils.GetAudioVolumeFromCamera();
        if(fadeTime > 0)
            _audioSource.DOFade(0.0f, fadeTime).OnComplete(() => _audioSource.enabled = false);

        EventManager.OnCameraOrthograficSizeChange += EventManager_OnCameraOrthograficSizeChange;
        EventManager_OnCameraOrthograficSizeChange(Camera.main.orthographicSize);
    }

    private void OnDestroy()
    {
        EventManager.OnCameraOrthograficSizeChange -= EventManager_OnCameraOrthograficSizeChange;
    }

    private void EventManager_OnCameraOrthograficSizeChange(float cameraOrthoSize)
    {
        if (maxOrthoDistance < Camera.main.orthographicSize)
            _audioSource.mute = true;
        else
            _audioSource.mute = false;

        _audioSource.volume = volumeMultiplier * Utils.GetAudioVolumeFromCamera(maxOrthoDistance);        
    }
}
