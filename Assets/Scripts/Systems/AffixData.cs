using UnityEngine;

public enum AffixStat
{
    PhysDamage, MagicDamage, Defense, HP, Mana,
    MoveSpeed, AttackSpeed, STR, VIT, INT, DEX,
    AllStats, MagicFind
}

public enum AffixTier { Common, Uncommon, Rare }

[CreateAssetMenu(fileName = "NewAffix", menuName = "RPG/Affix Data")]
public class AffixData : ScriptableObject
{
    [Header("Identificacion")]
    public string    prefix    = "";     // prefijo de nombre ("Afilado", "Arcano", etc.)
    public AffixStat stat;
    public AffixTier tier      = AffixTier.Common;

    [Header("Valor (se elige random entre min y max)")]
    public float     valueMin  = 1f;
    public float     valueMax  = 5f;

    [Header("Restricciones")]
    public ItemType  allowedType = ItemType.Weapon;  // para qué tipo de item aplica
    public bool      anyType     = false;            // si true, aplica a todos los tipos

    public float RollValue() => Random.Range(valueMin, valueMax);

    public static AffixData[] AllAffixes;  // se llena desde AffixPool al iniciar
}
