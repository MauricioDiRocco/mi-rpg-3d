using UnityEngine;
using System;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int      amount;

    public bool IsEmpty => item == null;
    public void Set(ItemData i, int qty) { item = i; amount = qty; }
    public void Clear()                  { item = null; amount = 0; }
}

public class Inventory : MonoBehaviour
{
    public int totalSlots = 48; // 6x8 como Metin2

    [HideInInspector] public int gold;
    [HideInInspector] public InventorySlot[] slots;

    public event Action OnInventoryChanged;
    public event Action OnGoldChanged;

    void Awake()
    {
        slots = new InventorySlot[totalSlots];
        for (int i = 0; i < totalSlots; i++)
            slots[i] = new InventorySlot();
    }

    public bool AddItem(ItemData item, int qty = 1)
    {
        // Intentar apilar primero si el item es apilable
        if (item.stackMax > 1)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item && slot.amount < item.stackMax)
                {
                    int space = item.stackMax - slot.amount;
                    int add   = Mathf.Min(space, qty);
                    slot.amount += add;
                    qty         -= add;
                    if (qty <= 0) { OnInventoryChanged?.Invoke(); return true; }
                }
            }
        }

        // Buscar slot vacío
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.Set(item, qty);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false; // inventario lleno
    }

    public bool RemoveItem(ItemData item, int qty = 1)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == item && slot.amount >= qty)
            {
                slot.amount -= qty;
                if (slot.amount <= 0) slot.Clear();
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public bool HasItem(ItemData item, int qty = 1)
    {
        foreach (var slot in slots)
            if (!slot.IsEmpty && slot.item == item && slot.amount >= qty) return true;
        return false;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke();
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke();
        return true;
    }
}
