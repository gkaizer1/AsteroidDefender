using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class LevelUnlockTile
{
    public string TileManagerName;
    public int row;
    public int column;
    public GameObject tilePrefab;
    public UnityEvent<GameObject> OnUnlocked;
}

public class UnlockTileBehavior : MonoBehaviour
{
    public List<LevelUnlockTile> tilesToUnlock = new List<LevelUnlockTile>();
    public UnityEvent OnUnlockCompleted;
    public bool zoomOnUnlock = true;

    bool _continue = false;
    Cinemachine.CinemachineVirtualCamera _camera;

    void Start()
    {
        _camera = this.GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    public void DisableZoom()
    {
        zoomOnUnlock = false;
    }

    public void Unlock()
    {
        StartCoroutine(StartUnlockLoop());
    }

    public void Continue()
    {
        _continue = true;
    }

    public IEnumerator StartUnlockLoop()
    {
        foreach (var tileUnlockConfig in tilesToUnlock)
        {
            if (GameObject.Find(tileUnlockConfig.TileManagerName) == null)
                continue;

            yield return new WaitForSeconds(1f);

            TileManagerBehavior tileManager = GameObject.Find(tileUnlockConfig.TileManagerName).GetComponent<TileManagerBehavior>();
            if (tileManager == null)
            {
                Debug.LogError($"Failed to file TileManager=[{tileManager}]");
                continue;
            }

            // Make sure the tile type is locked - DO NOT OVERRIDE existing tiles
            var existingTile = tileManager.GetTile(tileUnlockConfig.row, tileUnlockConfig.column);
            if (existingTile != null)
            {
                var tileBehavior = existingTile.GetComponent<TileBehavior>();
                if (tileBehavior != null)
                {
                    if (tileBehavior.tileType != TileType.LOCKED)
                    {
                        tileUnlockConfig.OnUnlocked?.Invoke(existingTile);
                        continue;
                    }

                }
            }
            var tile = tileManager.AddTile(
                tileUnlockConfig.row,
                tileUnlockConfig.column,
                tileUnlockConfig.tilePrefab,
                true /* overwrite-existing */);

            if (tile.GetComponent<Animator>() != null)
            {
                tile.GetComponent<Animator>().enabled = true;
                tile.GetComponent<Animator>().SetTrigger("tile_unlocked");
            }

            if (zoomOnUnlock)
            {
                var popup = UIPopup.GetPopup("Popup - Tile Unlocked");
                var tilePopup = popup.Canvas.GetComponentInChildren<PopupTileUnlockBehavior>();
                tilePopup.OnDestroyEvent += () =>
                {
                    _continue = true;
                };
                UIPopupManager.AddToQueue(popup);
            }

            /*
             * Create a tmp object for the camera to follow
             */
            var tmp_object = new GameObject();
            tmp_object.name = $"follow_{tile.name}";
            tmp_object.transform.position = tile.transform.position + Vector3.up * 1.0f;

            bool unlockCamActive = false;
            if (zoomOnUnlock)
            {
                unlockCamActive = true;
                _camera.Follow = tmp_object.transform;
                _camera.enabled = true;
            }

            try
            {
                tileUnlockConfig.OnUnlocked?.Invoke(tile);
            }
            catch
            { }

            if (unlockCamActive)
            {
                _camera.enabled = false;
            }

            // Wait for a signal to unlock next time
            while (!_continue && zoomOnUnlock)
                yield return new WaitForSeconds(0.1f);

            Destroy(tmp_object);
            _continue = false;
        }

        OnUnlockCompleted?.Invoke();

        yield return null;
    }
}
