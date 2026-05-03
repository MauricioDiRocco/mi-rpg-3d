using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Panel de crafteo (tecla K). Muestra recetas disponibles.
public class CraftingUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject       panel;

    [Header("Recetas disponibles")]
    public CraftingRecipe[] recipes;
    public Transform        recipeListContainer;
    public GameObject       recipeRowPrefab;

    [Header("Detalle de receta seleccionada")]
    public TextMeshProUGUI  recipeNameText;
    public TextMeshProUGUI  ingredientsText;
    public TextMeshProUGUI  resultText;
    public TextMeshProUGUI  yangCostText;
    public Button           craftButton;
    public TextMeshProUGUI  feedbackText;

    [Header("Tooltip")]
    public ItemTooltipUI    tooltip;

    private CraftingSystem  craftingSystem;
    private CraftingRecipe  selectedRecipe;

    void Start()
    {
        var player     = GameObject.FindGameObjectWithTag("Player");
        craftingSystem = player?.GetComponent<CraftingSystem>();

        if (craftingSystem != null)
            craftingSystem.OnCraftingChanged += Refresh;

        panel?.SetActive(false);
        craftButton?.onClick.AddListener(TryCraft);
        BuildRecipeList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            panel?.SetActive(!panel.activeSelf);

        if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            panel.SetActive(false);
    }

    void BuildRecipeList()
    {
        if (recipeListContainer == null || recipeRowPrefab == null) return;

        foreach (Transform child in recipeListContainer)
            Destroy(child.gameObject);

        foreach (var recipe in recipes)
        {
            var go  = Instantiate(recipeRowPrefab, recipeListContainer);
            var btn = go.GetComponent<Button>();
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();

            if (txt != null)
            {
                txt.text  = recipe.result.itemName;
                txt.color = recipe.result.RarityColor();
            }

            var captured = recipe;
            btn?.onClick.AddListener(() => SelectRecipe(captured));
        }
    }

    void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;

        if (recipeNameText  != null) recipeNameText.text  = recipe.result.itemName;
        if (resultText      != null) resultText.text      = $"Resultado: {recipe.result.itemName} x{recipe.resultAmount}";
        if (yangCostText    != null) yangCostText.text    = recipe.yangCost > 0 ? $"Costo: {recipe.yangCost} Yang" : "";

        if (ingredientsText != null)
        {
            var sb = new System.Text.StringBuilder("Ingredientes:\n");
            foreach (var ing in recipe.ingredients)
                sb.AppendLine($"  • {ing.item.itemName} x{ing.amount}");
            ingredientsText.text = sb.ToString();
        }

        bool canCraft = craftingSystem != null && craftingSystem.CanCraft(recipe);
        if (craftButton != null) craftButton.interactable = canCraft;
        if (feedbackText != null) feedbackText.text = canCraft ? "" : GetCantCraftReason(recipe);

        tooltip?.Show(recipe.result, recipeNameText?.transform.position ?? Vector3.zero);
    }

    string GetCantCraftReason(CraftingRecipe recipe)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var pStats = player?.GetComponent<PlayerStats>();
        var inv    = player?.GetComponent<Inventory>();

        if (pStats != null && pStats.level < recipe.requiredLevel)
            return $"Nivel insuficiente (req. {recipe.requiredLevel})";
        if (inv != null && inv.gold < recipe.yangCost)
            return $"Sin Yang ({recipe.yangCost} necesarios)";
        return "Ingredientes insuficientes";
    }

    void TryCraft()
    {
        if (selectedRecipe == null || craftingSystem == null) return;

        if (craftingSystem.Craft(selectedRecipe))
        {
            if (feedbackText != null) feedbackText.text = $"¡{selectedRecipe.result.itemName} creado!";
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "No se pudo craftear";
        }

        SelectRecipe(selectedRecipe);
    }

    void Refresh()
    {
        if (selectedRecipe != null) SelectRecipe(selectedRecipe);
    }

    void OnDestroy()
    {
        if (craftingSystem != null)
            craftingSystem.OnCraftingChanged -= Refresh;
    }
}
