using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LevelPopupBehavior : MonoBehaviour
{
    public DiffcultyIndictorBehavior difficultyBehavior;
    public TextMeshProUGUI levelLabelText;
    public List<AstroidSpawnerBehavior> astroidSpawners = new List<AstroidSpawnerBehavior>();
    public AstroidPanelBehavior astroidPanel;

    public int difficulty;
    public string levelLabel;

    public event System.Action OnDestroyed;

    void Start()
    {
        levelLabelText.text = levelLabel;
        difficultyBehavior.difficulty = difficulty;
        difficultyBehavior.UpdateStars();

        Dictionary<Sprite, int> asteroidImageCounts = new Dictionary<Sprite, int>();
        foreach(var spawner in astroidSpawners)
        {
            foreach (var asteroidSetting in spawner.Enemies)
            {
                if (!asteroidImageCounts.ContainsKey(asteroidSetting.Icon))
                {
                    asteroidImageCounts.Add(asteroidSetting.Icon, asteroidSetting.count);
                }
                else
                {
                    asteroidImageCounts[asteroidSetting.Icon] += asteroidSetting.count;

                }
            }
        }

        foreach (var keyval in asteroidImageCounts)
        {
            astroidPanel.AddAstroid(keyval.Key, keyval.Value);
        }
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
