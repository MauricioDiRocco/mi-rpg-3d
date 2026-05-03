using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int strength     = 10;
    public int vitality     = 10;
    public int intelligence = 10;
    public int dexterity    = 10;

    [Header("Level")]
    public int level      = 1;
    public int statPoints = 0;

    // Equipo se busca en Awake para incluir sus bonuses en los stats derivados
    private EquipmentManager equipment;

    // Stats derivados (incluyen bonuses de equipo)
    public float MaxHP       => 120f + vitality     * 18f + (equipment?.BonusHP          ?? 0f);
    public float MaxMana     => 60f  + intelligence * 12f + (equipment?.BonusMana        ?? 0f);
    public float PhysDamage  => 4f   + strength     * 1.2f  + (equipment?.BonusPhysDamage  ?? 0f);
    public float MagicDamage => intelligence * 2.2f        + (equipment?.BonusMagicDamage ?? 0f);
    public float Defense     => vitality * 1.5f + dexterity * 0.5f + (equipment?.BonusDefense ?? 0f);
    public float MoveSpeed   => 5.2f + dexterity * 0.07f  + (equipment?.BonusMoveSpeed   ?? 0f);
    public float AttackSpeed => 0.8f + dexterity * 0.015f + (equipment?.BonusAttackSpeed ?? 0f);

    [HideInInspector] public float currentHP;
    [HideInInspector] public float currentMana;

    public event Action OnStatsChanged;
    public event Action OnDeath;
    public event Action OnHit;   // dispara GetHit en el animator

    public void NotifyStatsChanged() => OnStatsChanged?.Invoke();

    void Awake()
    {
        equipment   = GetComponent<EquipmentManager>();
        currentHP   = MaxHP;
        currentMana = MaxMana;
    }

    public void TakeDamage(float rawDamage)
    {
        float mitigated = Mathf.Max(1f, rawDamage - Defense * 0.3f);
        currentHP = Mathf.Clamp(currentHP - mitigated, 0f, MaxHP);

        FloatingDamageSpawner.Spawn(transform.position, mitigated, DamageType.Physical);
        OnHit?.Invoke();

        if (currentHP <= 0f) OnDeath?.Invoke();
    }

    public void HealHP(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0f, MaxHP);
        FloatingDamageSpawner.Spawn(transform.position, amount, DamageType.Heal);
    }

    public void UseMana(float amount)
    {
        currentMana = Mathf.Clamp(currentMana - amount, 0f, MaxMana);
    }

    public void AddStatPoint(string stat)
    {
        if (statPoints <= 0) return;
        statPoints--;
        switch (stat)
        {
            case "STR": strength++;     break;
            case "VIT": vitality++;     break;
            case "INT": intelligence++; break;
            case "DEX": dexterity++;    break;
        }
        currentHP   = Mathf.Min(currentHP,   MaxHP);
        currentMana = Mathf.Min(currentMana, MaxMana);
        OnStatsChanged?.Invoke();
    }
}
