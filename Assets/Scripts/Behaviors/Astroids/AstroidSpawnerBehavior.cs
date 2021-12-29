using DG.Tweening;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class EnemySpawnSetting
{
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public Sprite Icon;

    [HideInInspector]
    public float timeSinceLastSpawn = 0.0f;
    public int count = 10;
    public float secondsBetweenSpawn = 1.0f;

    public enum SpawnType { SINGLE, ALL_IN_ORBIT };
}

public class AstroidSpawnerBehavior : MonoBehaviour
{
    public bool spawnOnAwake = true;

    [FormerlySerializedAs("astroidSettings")]
    public List<EnemySpawnSetting> Enemies = new List<EnemySpawnSetting>();

    [Header("SpawnSettings")]
    public string enemyContainerName = "ENEMIES_CONTAINER";
    public float initialSpawnDelay = 1f;

    public float spawnRadius = 3f;

    [Header("Callbacks")]
    public UnityEvent<AstroidSpawnerBehavior> OnLevelCompleted;
    public UnityEvent<GameObject> OnEnemySpawned;
    public UnityEvent OnStartedSpawning;
    public UnityEvent OnSpawnerInitialized;
    public UnityEvent OnSpawnCompleted;

    int co_spawners = 0;
    List<GameObject> _enemiesSpawned = new List<GameObject>();
    GameObject _mask = null;
    GameObject _enemyContainer = null;

    [Header("Prefabs")]
    public GameObject WorldTextPrefab = null;
    GameObject _worldTextInstance = null;
    int _enemiesToSpawn = 0;

    private void Start()
    {
        // Compute total enemies to spawn
        _enemiesToSpawn = 0;
        Enemies.ForEach(x => _enemiesToSpawn += x.count);

        _enemyContainer = GameObject.Find("ENEMIES_CONTAINER");

        OnSpawnerInitialized?.Invoke();
        if (spawnOnAwake)
            StartSpawningAstroids();
    }


    private void OnDestroy()
    {
        Destroy(_mask);
        if(_worldTextInstance != null)
            Destroy(_worldTextInstance);
    }

    void  CloseSpawnGate()
    {
        _worldTextInstance.SetActive(false);
    }

    public void ShowEnemiesText()
    {
        _worldTextInstance = Instantiate(WorldTextPrefab);
        if (_worldTextInstance == null)
            return;

        float x = this.transform.position.x;
        float y = this.transform.position.y + (spawnRadius / 2.0f) + 1;

        CanvasScreenBehavior behavior = _worldTextInstance.GetComponentInChildren<CanvasScreenBehavior>();
        behavior.position = new Vector3(x, y, 1f);
        behavior.Text = $"x{_enemiesToSpawn}";
        if (Enemies != null && Enemies.Any())
            behavior.Icon = Enemies[0].Icon;
        _worldTextInstance.SetActive(true);        
    }

    public void StartSpawningEnemies()
    {
        OnStartedSpawning?.Invoke();

        foreach (var setting in Enemies)
        {
            StartCoroutine(EnemySpawnerCoRoutine(setting));            
        }

        /*
         * Start co-routine to check if this spwaner has completed
         * to inform listeners (and close spawn gate !)
         */
        StartCoroutine(co_LevelCompletionChecker());
    }

    public void StartSpawningAstroids()
    {
        StartSpawningEnemies();
    }

    IEnumerator co_LevelCompletionChecker()
    {

        while (true)
        {
            if (co_spawners > 0 || _enemiesSpawned.Exists(x => x != null))
                yield return new WaitForSeconds(1.0f);
            else
            {
                OnLevelCompleted.Invoke(this);
                break;
            }
        }

    }

    IEnumerator EnemySpawnerCoRoutine(EnemySpawnSetting settings)
    {
        // Increment spwaner count
        co_spawners++;

        yield return new WaitForSeconds(initialSpawnDelay);

        int spawnCount = settings.count;
        for(int i = spawnCount; i > 0; i--)
        {
            yield return new WaitForSeconds(settings.secondsBetweenSpawn);
            SpawnEnemy(settings);
        }

        // Wait for the last enemy to spawn
        yield return new WaitForSeconds(3.0f);
        CloseSpawnGate();
        OnSpawnCompleted?.Invoke();
        yield return new WaitForSeconds(2.0f);

        // Decrement spwaner count
        co_spawners--;
    }

    public void SpawnEnemy(EnemySpawnSetting settings)
    {
        if (AstroidsContainer.Instance == null)
            return;

        _enemiesToSpawn--;
        if(_worldTextInstance != null)
            _worldTextInstance.GetComponent<CanvasScreenBehavior>().Text = $"x{_enemiesToSpawn.ToString()}";

        float xStartPos = this.transform.position.x;
        float yStartPos = this.transform.position.y;

        GameObject enemy = Instantiate(settings.enemyPrefab, _enemyContainer == null ? null : _enemyContainer.transform);
        enemy.transform.position = new Vector3(xStartPos, yStartPos, 1.0f);
        
        _enemiesSpawned.Add(enemy);

        OnEnemySpawned?.Invoke(enemy);
    }
}
