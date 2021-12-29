using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelUI : MonoBehaviour
{
    [Range(0, 5)]
    public int DifficultyLevel = 4;

    public int level = 1;
    public string NextScene = "Level 2";

    public List<AstroidSpawnerBehavior> enemySpawners = new List<AstroidSpawnerBehavior>();
    int completedSpawners = 0;
    public bool showLevelUI = true;
    public bool endLevelWhenEnemiesDie = true;

    public static string CurrentLevel;

    [Header("Callbacks")]
    public UnityEvent OnUIShown;
    public UnityEvent OnLevelFinished;

    // Start is called before the first frame update
    void Start()
    {
        GlobalGameSettings.Instance.LevelStart(level);
        CurrentLevel = this.gameObject.scene.name;

        var spawners = GameObject.FindObjectsOfType<AstroidSpawnerBehavior>();
        foreach(var spawner in spawners)
        {
            if (enemySpawners.Contains(spawner))
                continue;

            enemySpawners.Add(spawner);
        }

        foreach(var asteroidSpawner in enemySpawners)
        {
            asteroidSpawner.OnLevelCompleted.AddListener(OnLevelCompleted);
        }

        if (showLevelUI)
        {
            ShowUI();
        }
        else
        {
            OnUIShown?.Invoke();
        }
    }

    private void OnDestroy()
    {
        foreach (var asteroidSpawner in enemySpawners)
        {
            asteroidSpawner.OnLevelCompleted.RemoveListener(OnLevelCompleted);
        }
    }

    public void OnLevelCompleted(AstroidSpawnerBehavior behavior)
    {
        completedSpawners++;
        if (completedSpawners == enemySpawners.Count)
        {
            completedSpawners = 0;
            OnLevelFinished?.Invoke();
            if (endLevelWhenEnemiesDie)
            {
                SceneManager.UnloadScene(this.gameObject.scene.name);
                SceneManager.LoadScene(NextScene, LoadSceneMode.Additive);
            }
        }
    }

    public void NextLevel()
    {
        SceneManager.UnloadScene(this.gameObject.scene.name);
        SceneManager.LoadScene(NextScene, LoadSceneMode.Additive);
    }

    public void ShowUI()
    {
        var popup = UIPopup.GetPopup("Popup - Level");
        var levelPopup = popup.Canvas.GetComponentInChildren<LevelPopupBehavior>();

        if (levelPopup != null)
        {
            levelPopup.difficulty = DifficultyLevel;
            levelPopup.levelLabel = $"Level {level}";
            levelPopup.astroidSpawners = enemySpawners;
        }
        levelPopup.OnDestroyed += () => { OnUIShown?.Invoke(); };
        UIPopupManager.AddToQueue(popup);
    }
}
