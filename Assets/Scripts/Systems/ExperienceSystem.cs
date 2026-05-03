using UnityEngine;
using System;

[RequireComponent(typeof(PlayerStats))]
public class ExperienceSystem : MonoBehaviour
{
    private PlayerStats stats;

    [HideInInspector] public long currentExp;

    public event Action      OnExpChanged;
    public event Action<int> OnLevelUp;   // nuevo nivel

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    // Curva exponencial similar a Metin2
    public long ExpRequired(int level)
    {
        return (long)(100 * Mathf.Pow(1.45f, level - 1));
    }

    public long ExpForNextLevel => ExpRequired(stats.level);

    public float ExpPercent => (float)currentExp / ExpForNextLevel;

    public void GainExp(long amount)
    {
        currentExp += amount;
        CheckLevelUp();
        OnExpChanged?.Invoke();
    }

    public void LoseExp(float percent)
    {
        long loss  = (long)(ExpForNextLevel * percent);
        currentExp = (long)Mathf.Max(0, currentExp - loss);
        OnExpChanged?.Invoke();
    }

    void CheckLevelUp()
    {
        while (currentExp >= ExpForNextLevel && stats.level < 99)
        {
            currentExp    -= ExpForNextLevel;
            stats.level++;
            stats.statPoints += 5;          // 5 puntos por nivel como Metin2
            stats.currentHP   = stats.MaxHP;
            stats.currentMana = stats.MaxMana;
            stats.NotifyStatsChanged();
            OnLevelUp?.Invoke(stats.level);
            Debug.Log($"[LevelUp] Nivel {stats.level}! +5 puntos de stat");
        }
    }
}
