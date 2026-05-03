using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// UI de la tienda del mercader.
// Panel con dos tabs: Comprar (items del NPC) y Vender (inventario del player).
public class MerchantUI : MonoBehaviour
{
    [Header("Panel raiz")]
    public GameObject     panel;

    [Header("Header")]
    public TextMeshProUGUI merchantNameText;
    public TextMeshProUGUI playerGoldText;

    [Header("Tab Comprar")]
    public Transform      buyContainer;
    public GameObject     shopItemRowPrefab; // Image + Name + Price + BuyBtn

    [Header("Tab Vender")]
    public Transform      sellContainer;
    public GameObject     sellItemRowPrefab;

    [Header("Tabs")]
    public Button         buyTab;
    public Button         sellTab;
    public GameObject     buyPanel;
    public GameObject     sellPanel;

    [Header("Tooltip")]
    public ItemTooltipUI  tooltip;

    private MerchantNPC   currentMerchant;
    private Inventory     playerInventory;
    private PlayerStats   playerStats;

    void Start()
    {
        panel?.SetActive(false);
        buyTab?.onClick.AddListener(()  => { buyPanel?.SetActive(true);  sellPanel?.SetActive(false); });
        sellTab?.onClick.AddListener(() => { buyPanel?.SetActive(false); sellPanel?.SetActive(true); });
    }

    public void Open(MerchantNPC merchant)
    {
        currentMerchant = merchant;

        var playerGO    = GameObject.FindGameObjectWithTag("Player");
        playerInventory = playerGO?.GetComponent<Inventory>();
        playerStats     = playerGO?.GetComponent<PlayerStats>();

        if (merchantNameText != null) merchantNameText.text = merchant.merchantName;
        panel?.SetActive(true);

        RefreshBuyList();
        RefreshSellList();
        RefreshGold();
    }

    public void Close()
    {
        panel?.SetActive(false);
        currentMerchant = null;
    }

    void RefreshBuyList()
    {
        ClearContainer(buyContainer);
        if (currentMerchant == null) return;

        foreach (var item in currentMerchant.shopItems)
        {
            int price = currentMerchant.GetBuyPrice(item);
            var row = BuildRow(buyContainer, buyItemRowPrefab: true, item, price, "Comprar", () => TryBuy(item, price));
            AddTooltip(row, item);
        }
    }

    void RefreshSellList()
    {
        ClearContainer(sellContainer);
        if (playerInventory == null || currentMerchant == null) return;

        foreach (var slot in playerInventory.slots)
        {
            if (slot.IsEmpty) continue;
            var capturedItem = slot.item;
            int price = currentMerchant.GetSellPrice(capturedItem);
            var row = BuildRow(sellContainer, buyItemRowPrefab: false, capturedItem, price, "Vender", () => TrySell(capturedItem, price));
            AddTooltip(row, capturedItem);
        }
    }

    GameObject BuildRow(Transform container, bool buyItemRowPrefab, ItemData item, int price, string btnLabel, System.Action onPress)
    {
        var prefab = buyItemRowPrefab ? this.buyItemRowPrefab : sellItemRowPrefab;
        if (prefab == null || container == null) return null;

        var go = Instantiate(prefab, container);
        var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 1) { texts[0].text = item.itemName; texts[0].color = item.RarityColor(); }
        if (texts.Length >= 2) texts[1].text = $"{price} Yang";

        var btn = go.GetComponentInChildren<Button>();
        btn?.onClick.AddListener(() => { onPress(); RefreshBuyList(); RefreshSellList(); RefreshGold(); });

        return go;
    }

    // Prefab field aliases
    private GameObject buyItemRowPrefab => shopItemRowPrefab;

    void TryBuy(ItemData item, int price)
    {
        if (playerInventory == null || playerStats == null) return;

        if (playerInventory.gold < price)
        {
            Debug.Log("[Merchant] Sin Yang suficiente");
            return;
        }
        if (!playerInventory.AddItem(item))
        {
            Debug.Log("[Merchant] Inventario lleno");
            return;
        }
        playerInventory.AddGold(-price);
        Debug.Log($"[Merchant] Comprado: {item.itemName} por {price} Yang");
    }

    void TrySell(ItemData item, int price)
    {
        if (playerInventory == null) return;
        playerInventory.RemoveItem(item);
        playerInventory.AddGold(price);
        Debug.Log($"[Merchant] Vendido: {item.itemName} por {price} Yang");
    }

    void RefreshGold()
    {
        if (playerGoldText != null && playerInventory != null)
            playerGoldText.text = $"Yang: {playerInventory.gold:N0}";
    }

    void AddTooltip(GameObject row, ItemData item)
    {
        if (row == null || tooltip == null) return;
        var trigger = row.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var enter = new UnityEngine.EventSystems.EventTrigger.Entry
            { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
        enter.callback.AddListener(_ => tooltip.Show(item, row.transform.position));

        var exit = new UnityEngine.EventSystems.EventTrigger.Entry
            { eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => tooltip.Hide());

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }

    void ClearContainer(Transform container)
    {
        if (container == null) return;
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    void Update()
    {
        if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }
}
