using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Panel de equipamiento estilo Metin2.
// slotIcons[i] corresponde al EquipSlot con índice i (Weapon=0, Armor=1, ... Amulet=7).
// slotLabels[i] se oculta cuando hay un item equipado.
public class EquipmentSlotsUI : MonoBehaviour
{
    public Image[]          slotIcons;   // 8 Images, índice = (int)EquipSlot
    public TextMeshProUGUI[] slotLabels; // 8 labels de placeholder ("ARMA", "PECHO", etc.)

    EquipmentManager equipment;

    static readonly string[] SLOT_NAMES =
        { "ARMA", "PECHO", "CASCO", "BOTAS", "GUANT.", "ANILLO", "ANILLO", "AMUL." };

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        equipment  = player?.GetComponent<EquipmentManager>();
        if (equipment != null) equipment.OnEquipmentChanged += Refresh;
        InitLabels();
        Refresh();
    }

    void OnDestroy()
    {
        if (equipment != null) equipment.OnEquipmentChanged -= Refresh;
    }

    void OnEnable() => Refresh();

    void InitLabels()
    {
        for (int i = 0; i < slotLabels?.Length && i < SLOT_NAMES.Length; i++)
            if (slotLabels[i] != null) slotLabels[i].text = SLOT_NAMES[i];
    }

    void Refresh()
    {
        if (equipment == null) return;
        for (int i = 0; i < 8; i++)
        {
            var item = equipment.GetEquipped((EquipSlot)i);
            bool has = item != null;

            if (slotIcons  != null && i < slotIcons.Length  && slotIcons[i]  != null)
            {
                slotIcons[i].enabled = has;
                if (has) { slotIcons[i].sprite = item.icon; slotIcons[i].color = item.RarityColor(); }
            }
            if (slotLabels != null && i < slotLabels.Length && slotLabels[i] != null)
                slotLabels[i].enabled = !has;
        }
    }

    // Llamar desde Button.onClick (idx = (int)EquipSlot)
    public void ClickSlot(int idx)
    {
        equipment?.Unequip((EquipSlot)idx, sendToInventory: true);
    }
}
