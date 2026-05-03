using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Ventana de inventario grid estilo Metin2 (tecla I).
// slotPrefab debe tener: Image "Icon", TextMeshProUGUI "Amount", Button.
public class InventoryUI : MonoBehaviour
{
    public Transform           slotsContainer; // GridLayoutGroup aquí
    public GameObject          slotPrefab;
    public TextMeshProUGUI     goldText;
    public ItemTooltipUI       tooltip;        // referencia al tooltip (opcional)

    private Inventory          inventory;
    private EquipmentManager   equipment;
    private GameObject[]       slotObjects;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        inventory  = player?.GetComponent<Inventory>();
        equipment  = player?.GetComponent<EquipmentManager>();

        if (inventory == null) return;
        inventory.OnInventoryChanged += RefreshSlots;
        inventory.OnGoldChanged      += RefreshGold;

        BuildGrid();
        RefreshSlots();
        RefreshGold();
    }

    void OnEnable()
    {
        RefreshSlots();
        RefreshGold();
    }

    void OnDestroy()
    {
        if (inventory == null) return;
        inventory.OnInventoryChanged -= RefreshSlots;
        inventory.OnGoldChanged      -= RefreshGold;
    }

    void BuildGrid()
    {
        if (slotPrefab == null || slotsContainer == null) return;

        slotObjects = new GameObject[inventory.totalSlots];
        for (int i = 0; i < inventory.totalSlots; i++)
        {
            var go  = Instantiate(slotPrefab, slotsContainer);
            go.name = $"Slot_{i}";
            slotObjects[i] = go;

            int idx = i; // captura para el closure
            var btn = go.GetComponent<Button>();
            btn?.onClick.AddListener(() => OnSlotClicked(idx));

            // Hover para tooltip
            var trigger = go.AddComponent<EventTrigger>();
            AddHoverEvents(trigger, idx);
        }
    }

    void RefreshSlots()
    {
        if (slotObjects == null) return;

        for (int i = 0; i < inventory.slots.Length && i < slotObjects.Length; i++)
        {
            var slot  = inventory.slots[i];
            var go    = slotObjects[i];

            var icon   = go.transform.Find("Icon")?.GetComponent<Image>();
            var amount = go.transform.Find("Amount")?.GetComponent<TextMeshProUGUI>();
            var bg     = go.GetComponent<Image>();

            if (slot.IsEmpty)
            {
                if (icon   != null) { icon.enabled = false; }
                if (amount != null) amount.text = "";
                if (bg     != null) bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            }
            else
            {
                if (icon != null)
                {
                    icon.enabled = true;
                    icon.sprite  = slot.item.icon;
                    icon.color   = slot.item.RarityColor();
                }
                if (amount != null)
                    amount.text = slot.amount > 1 ? slot.amount.ToString() : "";
                // Borde de rareza
                if (bg != null)
                    bg.color = new Color(
                        slot.item.RarityColor().r * 0.3f,
                        slot.item.RarityColor().g * 0.3f,
                        slot.item.RarityColor().b * 0.3f,
                        0.9f
                    );
            }
        }
    }

    void RefreshGold()
    {
        if (goldText != null && inventory != null)
            goldText.text = $"{inventory.gold:N0} Yang";
    }

    void OnSlotClicked(int idx)
    {
        if (inventory.slots[idx].IsEmpty) return;

        var item = inventory.slots[idx].item;

        // Doble click = equipar si es equipable
        if (item.type is ItemType.Weapon or ItemType.Armor or ItemType.Helmet
                      or ItemType.Boots  or ItemType.Gloves or ItemType.Ring or ItemType.Amulet)
        {
            equipment?.Equip(item);
        }
        else if (item.type == ItemType.Potion)
        {
            var pStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.HealHP(item.healHP);
                pStats.currentMana = Mathf.Min(pStats.currentMana + item.healMana, pStats.MaxMana);
                inventory.RemoveItem(item);
            }
        }
    }

    void AddHoverEvents(EventTrigger trigger, int idx)
    {
        var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener((_) =>
        {
            if (!inventory.slots[idx].IsEmpty)
                tooltip?.Show(inventory.slots[idx].item, slotObjects[idx].transform.position);
        });

        var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener((_) => tooltip?.Hide());

        trigger.triggers.Add(enterEntry);
        trigger.triggers.Add(exitEntry);
    }
}
