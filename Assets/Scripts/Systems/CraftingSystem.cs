using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    private Inventory    inventory;
    private PlayerStats  stats;

    public event System.Action OnCraftingChanged;

    void Awake()
    {
        inventory = GetComponent<Inventory>();
        stats     = GetComponent<PlayerStats>();
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        if (stats.level < recipe.requiredLevel) return false;
        if (inventory.gold < recipe.yangCost)   return false;

        foreach (var ing in recipe.ingredients)
        {
            int count = CountItem(ing.item);
            if (count < ing.amount) return false;
        }
        return true;
    }

    public bool Craft(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe)) return false;

        // Consumir ingredientes
        foreach (var ing in recipe.ingredients)
        {
            for (int i = 0; i < ing.amount; i++)
                inventory.RemoveItem(ing.item);
        }

        // Cobrar yang
        if (recipe.yangCost > 0)
            inventory.AddGold(-recipe.yangCost);

        // Agregar resultado
        inventory.AddItem(recipe.result, recipe.resultAmount);

        Debug.Log($"[Crafting] Creado: {recipe.result.itemName} x{recipe.resultAmount}");
        OnCraftingChanged?.Invoke();
        return true;
    }

    int CountItem(ItemData item)
    {
        int total = 0;
        foreach (var slot in inventory.slots)
        {
            if (!slot.IsEmpty && slot.item == item)
                total += slot.amount;
        }
        return total;
    }
}
