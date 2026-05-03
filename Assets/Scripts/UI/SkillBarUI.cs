using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SkillSlotUI
{
    public Image              background;
    public Image              iconImage;
    public Image              cooldownOverlay; // Image negro semitransparente con Fill=Radial360
    public TextMeshProUGUI    cooldownText;
    public TextMeshProUGUI    keyLabel;        // "F1", "F2", etc.

    [HideInInspector] public float cooldownMax;
    [HideInInspector] public float cooldownRemaining;
    [HideInInspector] public bool  isOnCooldown;
}

// Hotbar de habilidades F1-F8, estilo Metin2.
// Posicionar en el centro-inferior del Canvas.
public class SkillBarUI : MonoBehaviour
{
    public SkillSlotUI[] slots = new SkillSlotUI[8];

    private static readonly KeyCode[] SkillKeys =
    {
        KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4,
        KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8
    };

    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].keyLabel != null)
                slots[i].keyLabel.text = $"F{i + 1}";
            SetCooldownOverlayVisible(slots[i], false);
        }
    }

    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            TickCooldown(slots[i]);

            if (Input.GetKeyDown(SkillKeys[i]))
                TryActivateSkill(i);
        }
    }

    void TickCooldown(SkillSlotUI slot)
    {
        if (!slot.isOnCooldown) return;

        slot.cooldownRemaining -= Time.deltaTime;

        if (slot.cooldownRemaining <= 0f)
        {
            slot.cooldownRemaining = 0f;
            slot.isOnCooldown      = false;
            SetCooldownOverlayVisible(slot, false);
        }
        else
        {
            float ratio = slot.cooldownRemaining / slot.cooldownMax;
            if (slot.cooldownOverlay != null) slot.cooldownOverlay.fillAmount = ratio;
            if (slot.cooldownText    != null) slot.cooldownText.text = Mathf.CeilToInt(slot.cooldownRemaining).ToString();
        }
    }

    // Llamar desde el sistema de skills para iniciar cooldown
    public void StartCooldown(int index, float duration)
    {
        if (index < 0 || index >= slots.Length) return;
        var slot              = slots[index];
        slot.cooldownMax       = duration;
        slot.cooldownRemaining = duration;
        slot.isOnCooldown      = true;
        SetCooldownOverlayVisible(slot, true);
    }

    // Asignar icono de skill a un slot (llamar al cambiar build)
    public void SetSlotIcon(int index, Sprite icon)
    {
        if (index < 0 || index >= slots.Length) return;
        if (slots[index].iconImage != null)
            slots[index].iconImage.sprite = icon;
    }

    void SetCooldownOverlayVisible(SkillSlotUI slot, bool visible)
    {
        if (slot.cooldownOverlay != null)
        {
            slot.cooldownOverlay.enabled   = visible;
            slot.cooldownOverlay.fillAmount = visible ? 1f : 0f;
        }
        if (slot.cooldownText != null)
            slot.cooldownText.text = "";
    }

    void TryActivateSkill(int index)
    {
        var slot = slots[index];
        if (slot.isOnCooldown)
        {
            Debug.Log($"Skill F{index + 1} en cooldown ({slot.cooldownRemaining:F1}s)");
            return;
        }
        // TODO: conectar con SkillSystem cuando se implemente
        Debug.Log($"[SkillBar] Activando habilidad en slot F{index + 1}");
    }
}
