using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public bool _isActive = true;
    public bool stopAfterSpawn = true;
    public float spawnSpeed = 0.4f;

    public bool IsActive
    {
        get
        {
            return _isActive;
        }
        set
        {
            _isActive = value;
            OnActiveChanged?.Invoke();
        }
    }
    public event System.Action OnActiveChanged;

    // Start is called before the first frame update
    void Start()
    {
        EnemyManager.Enemies.Add(this.gameObject);
    }

    private void OnDestroy()
    {
        EnemyManager.Enemies.Remove(this.gameObject);
    }
}
