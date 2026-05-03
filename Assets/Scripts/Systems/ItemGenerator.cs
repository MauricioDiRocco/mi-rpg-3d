using System.Collections.Generic;
using UnityEngine;

// Genera items Magic/Rare/Unique con affixes estilo Diablo 2.
// Los stats se escalan con itemLevel (18% por nivel).
// reqLevel del item = itemLevel → el player necesita ese nivel para equiparlo.
public static class ItemGenerator
{
    static readonly int[] MAGIC_AFFIXES = { 1, 2 };
    static readonly int[] RARE_AFFIXES  = { 3, 6 };

    public static ItemData GenerateItem(ItemData baseItem, ItemRarity rarity,
        AffixData[] affixPool, float magicFind = 1f, int itemLevel = 1)
    {
        float levelScale = 1f + (itemLevel - 1) * 0.18f;  // +18% stats por nivel

        if (rarity == ItemRarity.Normal || affixPool == null || affixPool.Length == 0)
        {
            if (itemLevel <= 1) return baseItem;
            // Normal pero nivelado: solo ajustar reqLevel
            var normalCopy    = Object.Instantiate(baseItem);
            normalCopy.reqLevel = itemLevel;
            normalCopy.buyPrice  = Mathf.RoundToInt(baseItem.buyPrice  * (1f + (itemLevel - 1) * 0.1f));
            normalCopy.sellPrice = Mathf.RoundToInt(baseItem.sellPrice * (1f + (itemLevel - 1) * 0.1f));
            return normalCopy;
        }

        var generated     = Object.Instantiate(baseItem);
        generated.rarity  = rarity;
        generated.reqLevel = itemLevel;
        generated.buyPrice  = Mathf.RoundToInt(generated.buyPrice  * (rarity == ItemRarity.Magic ? 2.5f : 5f));
        generated.sellPrice = Mathf.RoundToInt(generated.sellPrice * (rarity == ItemRarity.Magic ? 2.5f : 5f));

        int affixCount = rarity == ItemRarity.Magic
            ? Random.Range(MAGIC_AFFIXES[0], MAGIC_AFFIXES[1] + 1)
            : Random.Range(RARE_AFFIXES[0],  RARE_AFFIXES[1]  + 1);

        var valid = new List<AffixData>();
        foreach (var a in affixPool)
            if (a.anyType || a.allowedType == baseItem.type)
                valid.Add(a);

        if (valid.Count == 0) return generated;

        var chosen = new HashSet<AffixData>();
        for (int i = 0; i < affixCount && chosen.Count < valid.Count; i++)
        {
            AffixData pick;
            int tries = 0;
            do { pick = valid[Random.Range(0, valid.Count)]; tries++; }
            while (chosen.Contains(pick) && tries < 20);

            if (chosen.Contains(pick)) continue;
            chosen.Add(pick);

            float val = pick.RollValue() * levelScale;
            ApplyAffix(generated, pick.stat, val);

            if (!string.IsNullOrEmpty(pick.prefix))
                generated.itemName = $"{pick.prefix} {generated.itemName}";
        }

        return generated;
    }

    static void ApplyAffix(ItemData item, AffixStat stat, float val)
    {
        switch (stat)
        {
            case AffixStat.PhysDamage:   item.bonusPhysDamage   += val;              break;
            case AffixStat.MagicDamage:  item.bonusMagicDamage  += val;              break;
            case AffixStat.Defense:      item.bonusDefense       += val;              break;
            case AffixStat.HP:           item.bonusHP            += val;              break;
            case AffixStat.Mana:         item.bonusMana          += val;              break;
            case AffixStat.MoveSpeed:    item.bonusMoveSpeed     += val * 0.01f;      break;
            case AffixStat.AttackSpeed:  item.bonusAttackSpeed   += val * 0.01f;      break;
            case AffixStat.STR:          item.bonusSTR           += Mathf.RoundToInt(val); break;
            case AffixStat.VIT:          item.bonusVIT           += Mathf.RoundToInt(val); break;
            case AffixStat.INT:          item.bonusINT           += Mathf.RoundToInt(val); break;
            case AffixStat.DEX:          item.bonusDEX           += Mathf.RoundToInt(val); break;
            case AffixStat.AllStats:
                int v = Mathf.RoundToInt(val);
                item.bonusSTR += v; item.bonusVIT += v;
                item.bonusINT += v; item.bonusDEX += v;
                break;
            case AffixStat.MagicFind:    break;
        }
    }

    public static ItemRarity RollRarity(float magicFind = 1f)
    {
        float uniqueChance = 0.5f  * magicFind;
        float rareChance   = 3f    * magicFind;
        float magicChance  = 12f   * magicFind;
        float roll         = Random.Range(0f, 100f);

        if (roll < Mathf.Min(uniqueChance, 15f))                         return ItemRarity.Unique;
        if (roll < Mathf.Min(uniqueChance + rareChance, 25f))            return ItemRarity.Rare;
        if (roll < Mathf.Min(uniqueChance + rareChance + magicChance, 50f)) return ItemRarity.Magic;
        return ItemRarity.Normal;
    }
}
