using UnityEngine;
using System;

public enum EquipSlot { Weapon, Armor, Helmet, Boots, Gloves, Ring1, Ring2, Amulet }

public class EquipmentManager : MonoBehaviour
{
    public ItemData[] equipped = new ItemData[8];

    private PlayerStats stats;
    private Inventory   inventory;

    // Bonuses totales del equipo (leídos por PlayerStats)
    public float BonusPhysDamage  { get; private set; }
    public float BonusMagicDamage { get; private set; }
    public float BonusDefense     { get; private set; }
    public float BonusHP          { get; private set; }
    public float BonusMana        { get; private set; }
    public float BonusMoveSpeed   { get; private set; }
    public float BonusAttackSpeed { get; private set; }
    public int   BonusSTR         { get; private set; }
    public int   BonusVIT         { get; private set; }
    public int   BonusINT         { get; private set; }
    public int   BonusDEX         { get; private set; }

    public event Action OnEquipmentChanged;

    void Awake()
    {
        stats     = GetComponent<PlayerStats>();
        inventory = GetComponent<Inventory>();
    }

    public bool Equip(ItemData item)
    {
        // Verificar requerimientos
        if (stats.level        < item.reqLevel ||
            stats.strength     < item.reqSTR   ||
            stats.dexterity    < item.reqDEX   ||
            stats.intelligence < item.reqINT)
        {
            Debug.Log("No cumplís los requisitos para equipar este ítem.");
            return false;
        }

        EquipSlot slot = GetSlotFor(item);

        if (equipped[(int)slot] != null)
            Unequip(slot, sendToInventory: true);

        inventory.RemoveItem(item);
        equipped[(int)slot] = item;
        Recalculate();
        OnEquipmentChanged?.Invoke();
        return true;
    }

    public void Unequip(EquipSlot slot, bool sendToInventory = true)
    {
        if (equipped[(int)slot] == null) return;
        if (sendToInventory)
            inventory.AddItem(equipped[(int)slot]);
        equipped[(int)slot] = null;
        Recalculate();
        OnEquipmentChanged?.Invoke();
    }

    void Recalculate()
    {
        BonusPhysDamage = BonusMagicDamage = BonusDefense = 0;
        BonusHP = BonusMana = BonusMoveSpeed = BonusAttackSpeed = 0;
        BonusSTR = BonusVIT = BonusINT = BonusDEX = 0;

        foreach (var it in equipped)
        {
            if (it == null) continue;
            BonusPhysDamage  += it.bonusPhysDamage;
            BonusMagicDamage += it.bonusMagicDamage;
            BonusDefense     += it.bonusDefense;
            BonusHP          += it.bonusHP;
            BonusMana        += it.bonusMana;
            BonusMoveSpeed   += it.bonusMoveSpeed;
            BonusAttackSpeed += it.bonusAttackSpeed;
            BonusSTR         += it.bonusSTR;
            BonusVIT         += it.bonusVIT;
            BonusINT         += it.bonusINT;
            BonusDEX         += it.bonusDEX;
        }

        stats.NotifyStatsChanged();
    }

    public ItemData GetEquipped(EquipSlot slot) => equipped[(int)slot];

    EquipSlot GetSlotFor(ItemData item) => item.type switch
    {
        ItemType.Weapon => EquipSlot.Weapon,
        ItemType.Armor  => EquipSlot.Armor,
        ItemType.Helmet => EquipSlot.Helmet,
        ItemType.Boots  => EquipSlot.Boots,
        ItemType.Gloves => EquipSlot.Gloves,
        ItemType.Ring   => equipped[(int)EquipSlot.Ring1] == null ? EquipSlot.Ring1 : EquipSlot.Ring2,
        ItemType.Amulet => EquipSlot.Amulet,
        _               => EquipSlot.Armor
    };
}
