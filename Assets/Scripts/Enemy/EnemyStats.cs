using UnityEngine;
using System;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP       = 80f;
    public float defense     = 3f;
    public float damage      = 8f;
    public float moveSpeed   = 2.5f;
    public float attackSpeed = 0.8f;    // ataques por segundo

    [Header("Recompensa")]
    public long  expReward   = 20;
    public int   itemLevel   = 1;       // nivel del loot que dropea; seteado por MobSpawner

    [HideInInspector] public float currentHP;

    public event Action               OnDeath;
    public event Action<float, float> OnHealthChanged;  // current, max

    public static event Action<string> OnAnyEnemyDied;  // para QuestSystem

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float rawDamage)
    {
        if (currentHP <= 0f) return;

        float mitigated = Mathf.Max(1f, rawDamage - defense * 0.3f);
        currentHP = Mathf.Clamp(currentHP - mitigated, 0f, maxHP);

        FloatingDamageSpawner.Spawn(transform.position, mitigated, DamageType.Physical);
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0f)
        {
            OnDeath?.Invoke();
            OnAnyEnemyDied?.Invoke(gameObject.tag);
        }
    }
}
