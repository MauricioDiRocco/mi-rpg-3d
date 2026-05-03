using UnityEngine;
using TMPro;

// NPC mercader. Interactuar con E (o click) para abrir tienda.
// Implementa IInteractable para que InteractionSystem lo detecte.
public class MerchantNPC : MonoBehaviour, IInteractable
{
    [Header("Inventario de la tienda")]
    public ItemData[] shopItems;

    [Header("Multiplicador de precio")]
    [Range(0.5f, 3f)]
    public float buyPriceMultiplier  = 1f;
    [Range(0.1f, 1f)]
    public float sellPriceMultiplier = 0.3f;

    [Header("Dialogo")]
    public string merchantName = "Mercader";
    public string greeting     = "¡Bienvenido! ¿Qué necesitás?";

    [Header("UI")]
    public TextMeshPro nameLabel;

    private MerchantUI merchantUI;
    private bool       playerInRange;

    // IInteractable
    public string InteractPrompt => $"Hablar con {merchantName}";

    void Start()
    {
        merchantUI = FindFirstObjectByType<MerchantUI>(FindObjectsInactive.Include);
        if (nameLabel != null) nameLabel.text = merchantName;
    }

    void Update()
    {
        if (playerInRange && Camera.main != null && nameLabel != null)
            nameLabel.transform.rotation = Camera.main.transform.rotation;
    }

    void OnMouseDown()
    {
        if (playerInRange) Interact();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { playerInRange = true; OnEnterRange(); }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { playerInRange = false; OnExitRange(); }
    }

    public void Interact()
    {
        if (merchantUI == null) return;
        merchantUI.Open(this);
        Debug.Log($"[{merchantName}] {greeting}");
    }

    public void OnEnterRange() { }
    public void OnExitRange()  { }

    public int GetBuyPrice(ItemData item)  => Mathf.RoundToInt(item.buyPrice  * buyPriceMultiplier);
    public int GetSellPrice(ItemData item) => Mathf.RoundToInt(item.sellPrice * sellPriceMultiplier);
}
