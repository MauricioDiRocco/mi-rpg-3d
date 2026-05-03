using UnityEngine;

public enum ItemType   { Weapon, Armor, Helmet, Boots, Gloves, Ring, Amulet, Potion, Quest, Gold }
public enum ItemRarity { Normal, Magic, Rare, Unique }
public enum WeaponType { Sword, Axe, Mace, Staff, Bow }

[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identificacion")]
    public string     itemName    = "Item";
    public string     description = "";
    public Sprite     icon;
    public ItemType   type        = ItemType.Armor;
    public ItemRarity rarity      = ItemRarity.Normal;

    [Header("Inventario")]
    public int slotsWidth  = 1;
    public int slotsHeight = 1;
    public int stackMax    = 1;  // >1 = apilable (pociones, gold)

    [Header("Requerimientos")]
    public int reqLevel = 1;
    public int reqSTR   = 0;
    public int reqDEX   = 0;
    public int reqINT   = 0;

    [Header("Bonuses de Stats")]
    public int   bonusSTR          = 0;
    public int   bonusVIT          = 0;
    public int   bonusINT          = 0;
    public int   bonusDEX          = 0;
    public float bonusHP           = 0;
    public float bonusMana         = 0;
    public float bonusPhysDamage   = 0;
    public float bonusMagicDamage  = 0;
    public float bonusDefense      = 0;
    public float bonusMoveSpeed    = 0;
    public float bonusAttackSpeed  = 0;

    [Header("Weapon (solo si type = Weapon)")]
    public WeaponType weaponType;
    public float      minDamage = 0;
    public float      maxDamage = 0;

    [Header("Pocion (solo si type = Potion)")]
    public float healHP   = 0;
    public float healMana = 0;

    [Header("Economia")]
    public int buyPrice  = 100;
    public int sellPrice = 20;

    [Header("Gold (solo si type = Gold)")]
    public int goldAmount = 0;

    // Color por rareza para UI
    public Color RarityColor()
    {
        return rarity switch
        {
            ItemRarity.Normal => Color.white,
            ItemRarity.Magic  => new Color(0.4f, 0.6f, 1f),
            ItemRarity.Rare   => Color.yellow,
            ItemRarity.Unique => new Color(0.85f, 0.45f, 0.1f),
            _                 => Color.white
        };
    }
}
