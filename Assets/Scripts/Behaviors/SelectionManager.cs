using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SelectionManager : MonoBehaviour
{
    private static SelectionManager _instance;
    private static readonly Object _lock = new Object();

    public delegate void SelectionChangeEventHandler(GameObject previous, GameObject current);
    public static event SelectionChangeEventHandler OnSelectedObjectChanged;

    public static event System.Action<GameObject> OnSelected;
    public static event System.Action<GameObject> OnSelectionLost;

    private static GameObject _selectedGameObject = null;

    public static SelectionManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Start()
    {
        Debug.Log($"Initializing SelectionManger {name}");
        _instance = this;
    }

    public static GameObject SelectedGameObject
    {
        get
        {
            if (Instance == null)
                return null;

            return _selectedGameObject;
        }
        set
        {
            if(value == _selectedGameObject)
            {
                return;
            }

            if (value != null)
                Debug.Log($"Selectd object=[{value.name}]");
            else
                Debug.Log("Selection changed to [None]");

            GameObject prev = _selectedGameObject;
            _selectedGameObject = value;
            OnSelectionLost?.Invoke(prev);
            OnSelected?.Invoke(_selectedGameObject);
            OnSelectedObjectChanged?.Invoke(prev, _selectedGameObject);
        }
    }
}
