using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootEntry
{
    public ItemData item;
    [Range(0f, 100f)] public float dropChance = 10f;
    public int minAmount = 1;
    public int maxAmount = 1;
}

[CreateAssetMenu(fileName = "LootTable", menuName = "RPG/Loot Table")]
public class LootTable : ScriptableObject
{
    [Header("Gold")]
    public bool  dropGold   = true;
    [Range(0f, 100f)] public float goldChance = 75f;
    public int   goldMin    = 5;
    public int   goldMax    = 50;

    [Header("Items")]
    public List<LootEntry> entries = new();

    // Hace el roll de todos los drops. magicFind multiplica las chances (estilo D2).
    public List<(ItemData item, int amount)> RollLoot(float magicFind = 1f)
    {
        var results = new List<(ItemData, int)>();

        foreach (var entry in entries)
        {
            if (entry.item == null) continue;

            // Magic find solo mejora items Magic+
            float multiplier = entry.item.rarity >= ItemRarity.Magic ? magicFind : 1f;
            float chance     = Mathf.Min(entry.dropChance * multiplier, 100f);

            if (Random.Range(0f, 100f) <= chance)
            {
                int qty = Random.Range(entry.minAmount, entry.maxAmount + 1);
                results.Add((entry.item, qty));
            }
        }

        return results;
    }

    public int RollGold()
    {
        if (!dropGold || Random.Range(0f, 100f) > goldChance) return 0;
        return Random.Range(goldMin, goldMax + 1);
    }
}
