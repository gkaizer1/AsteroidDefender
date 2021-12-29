using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnlockSatelliteBehavior : MonoBehaviour
{
    public int OrbitIndex = 0;
    public int Satellite = 0;
    Cinemachine.CinemachineVirtualCamera _camera;
    public UnityEvent OnSatelliteUnlock;
    public bool unlockOnSpawn = true;

    void Start()
    {
        _camera = this.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        if (unlockOnSpawn)
            Unlock();
    }

    public void Unlock()
    {
        StartCoroutine(co_UnlockSatellite());
    }

    public IEnumerator co_UnlockSatellite()
    {
        var orbits = GameObject.FindGameObjectsWithTag("orbit");
        foreach (var orbit in orbits)
        {
            var orbitBehavior = orbit.GetComponent<OrbitBehavior>();
            if (orbitBehavior == null)
                continue;

            if (orbitBehavior.orbitIndex != OrbitIndex)
                continue;

            var tile = orbitBehavior.UnlockSatellite(Satellite);

            bool _continue = false; ;
            var popup = UIPopup.GetPopup("popup_sattelite_unlock");
            var tilePopup = popup.Canvas.GetComponentInChildren<PopupTileUnlockBehavior>();
            tilePopup.OnDestroyEvent += () =>
            {
                _continue = true;
            };
            UIPopupManager.AddToQueue(popup);

            _camera.Follow = tile.transform;
            _camera.enabled = true;

            // Wait for a signal to unlock next time
            while (!_continue)
                yield return new WaitForSeconds(0.1f);

            _camera.enabled = false;

            yield return new WaitForSeconds(0.1f);
        }
        OnSatelliteUnlock?.Invoke();
    }
}
