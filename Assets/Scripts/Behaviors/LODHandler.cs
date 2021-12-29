using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

[System.Serializable]
public class LODInfo
{
    public GameObject Object;
    public float MinOrtho = 0.0f;
    public float MaxOrtho = float.MaxValue;
}

[ExecuteInEditMode]
public class LODHandler : MonoBehaviour
{
    public List<LODInfo> LOD = new List<LODInfo>();

    private void OnDisable()
    {
        LOD.ForEach(x => DisableAllChildren(x.Object));
    }

    private void DisableAllChildren(GameObject obj)
    {
        if (obj == null)
            return;

        obj.SetActive(false);
        foreach (Transform child in obj.transform)
        {
            DisableAllChildren(child.gameObject);
        }
    }
    private void EnableAllChildren(GameObject obj)
    {
        if (obj == null)
            return;

        obj.SetActive(true);
        foreach (Transform child in obj.transform)
        {
            EnableAllChildren(child.gameObject);
        }
    }

    private void OnEnable()
    {
        LOD.ForEach(x => EnableAllChildren(x.Object));

        OnCameraOrhtoChangedChange(Camera.main.orthographicSize);
        UpdateLod();
    }

    void Start()
    {
        EventManager.OnCameraOrthograficSizeChange += OnCameraOrhtoChangedChange;
        UpdateLod();
    }

    void UpdateLod()
    {
        OnCameraOrhtoChangedChange(Camera.main.orthographicSize);
    }

    private void OnCameraOrhtoChangedChange(float orthoSize)
    {
        LOD.ForEach(x =>
        {
            if (x?.Object == null)
                return;

            if (x.Object.GetComponent<SpriteRenderer>() == null)
                return;

            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            if (orthoSize >= x.MinOrtho && orthoSize < x.MaxOrtho)
            {
                x.Object.GetComponent<SpriteRenderer>().DOFade(1.0f, Random.Range(0.3f, 0.6f));
                x.Object.SetActive(true);
            }
            else
            {
                if (x.Object.activeSelf)
                {
                    x.Object.GetComponent<SpriteRenderer>().DOFade(0.0f, Random.Range(0.3f, 0.6f)).OnComplete(() =>
                    {
                        x.Object.SetActive(false);
                    });
                }
            }
        });
    }
}