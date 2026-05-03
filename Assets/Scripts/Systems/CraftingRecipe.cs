using UnityEngine;

[System.Serializable]
public class CraftingIngredient
{
    public ItemData item;
    public int      amount = 1;
}

[CreateAssetMenu(fileName = "NewRecipe", menuName = "RPG/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Resultado")]
    public ItemData              result;
    public int                  resultAmount = 1;

    [Header("Ingredientes")]
    public CraftingIngredient[] ingredients;

    [Header("Requerimientos")]
    public int requiredLevel     = 1;
    public int yangCost          = 0;

    [Header("Categoria")]
    public string category       = "General"; // "Armas", "Armadura", "Pociones", etc.
}
