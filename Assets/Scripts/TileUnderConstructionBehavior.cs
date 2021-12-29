using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileUnderConstructionBehavior : MonoBehaviour
{
    public UnityEvent<GameObject> OnConstructionCompleted;

    public GameObject TileToBuild;
    public float constructionTime = 5.0f;
    private float _elapsedTime = 0.0f;

    [Header("Children")]
    public CircleBehavior constructionCircle = null;

    public bool IsBuilding = true;
    public bool RequiresShuttle = false;
    private void Awake()
    {
        constructionCircle.MaxAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsBuilding)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= constructionTime)
        {
            GameObject newTile = null;
            var tileManager = this.GetComponent<TileBehavior>().TileManager;
            if (tileManager != null)
            {
                var tileInfo = tileManager.GetTileInfo(this.gameObject);
                newTile = tileManager.AddTile(tileInfo.row, tileInfo.col, TileToBuild, true);
            }
            else
            {
                newTile = Instantiate(TileToBuild, transform.parent);
                newTile.transform.localPosition = this.transform.localPosition;
                newTile.transform.localRotation = this.transform.localRotation;
            }

            OnConstructionCompleted?.Invoke(newTile.gameObject);

            if (this.gameObject != null)
                Destroy(this.gameObject);
        }
        else
        {
            constructionCircle.MaxAngle = _elapsedTime / constructionTime * 360.0f;
        }
    }
}
