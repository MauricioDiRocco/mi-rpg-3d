using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Tooltip que aparece al hacer hover sobre un item en el inventario.
// Panel que sigue la posición del slot con info del item.
public class ItemTooltipUI : MonoBehaviour
{
    public GameObject          panel;
    public TextMeshProUGUI     itemName;
    public TextMeshProUGUI     rarityText;
    public TextMeshProUGUI     typeText;
    public TextMeshProUGUI     statsText;
    public TextMeshProUGUI     reqText;
    public TextMeshProUGUI     descText;
    public TextMeshProUGUI     priceText;
    public RectTransform       rectTransform;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Show(ItemData item, Vector3 anchorWorldPos)
    {
        if (item == null || panel == null) return;

        panel.SetActive(true);

        // Posicionar el tooltip cerca del slot
        if (rectTransform != null)
            rectTransform.position = anchorWorldPos + new Vector3(160f, 0f, 0f);

        if (itemName  != null)
        {
            itemName.text  = item.itemName;
            itemName.color = item.RarityColor();
        }

        if (rarityText != null)
            rarityText.text = item.rarity.ToString();

        if (typeText != null)
            typeText.text = item.type.ToString();

        if (statsText != null)
            statsText.text = BuildStatsString(item);

        if (reqText != null)
            reqText.text = BuildReqString(item);

        if (descText != null)
            descText.text = item.description;

        if (priceText != null)
            priceText.text = $"Precio: {item.sellPrice} Yang";
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    string BuildStatsString(ItemData item)
    {
        var sb = new System.Text.StringBuilder();

        if (item.type == ItemType.Weapon)
            sb.AppendLine($"Daño: {item.minDamage:F0} - {item.maxDamage:F0}");

        if (item.bonusHP          != 0) sb.AppendLine($"+{item.bonusHP:F0} HP");
        if (item.bonusMana        != 0) sb.AppendLine($"+{item.bonusMana:F0} Mana");
        if (item.bonusPhysDamage  != 0) sb.AppendLine($"+{item.bonusPhysDamage:F0} Daño Físico");
        if (item.bonusMagicDamage != 0) sb.AppendLine($"+{item.bonusMagicDamage:F0} Daño Mágico");
        if (item.bonusDefense     != 0) sb.AppendLine($"+{item.bonusDefense:F0} Defensa");
        if (item.bonusSTR         != 0) sb.AppendLine($"+{item.bonusSTR} FUE");
        if (item.bonusVIT         != 0) sb.AppendLine($"+{item.bonusVIT} VIT");
        if (item.bonusINT         != 0) sb.AppendLine($"+{item.bonusINT} INT");
        if (item.bonusDEX         != 0) sb.AppendLine($"+{item.bonusDEX} DES");
        if (item.bonusAttackSpeed != 0) sb.AppendLine($"+{item.bonusAttackSpeed:F2} Vel. Ataque");
        if (item.bonusMoveSpeed   != 0) sb.AppendLine($"+{item.bonusMoveSpeed:F2} Vel. Movimiento");

        if (item.healHP   > 0) sb.AppendLine($"Restaura {item.healHP:F0} HP");
        if (item.healMana > 0) sb.AppendLine($"Restaura {item.healMana:F0} Mana");

        return sb.ToString().TrimEnd();
    }

    string BuildReqString(ItemData item)
    {
        var sb = new System.Text.StringBuilder();
        if (item.reqLevel > 1)  sb.AppendLine($"Req. Nivel: {item.reqLevel}");
        if (item.reqSTR   > 0)  sb.AppendLine($"Req. FUE:   {item.reqSTR}");
        if (item.reqDEX   > 0)  sb.AppendLine($"Req. DES:   {item.reqDEX}");
        if (item.reqINT   > 0)  sb.AppendLine($"Req. INT:   {item.reqINT}");
        return sb.ToString().TrimEnd();
    }
}
