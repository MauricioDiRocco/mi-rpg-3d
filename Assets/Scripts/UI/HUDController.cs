using UnityEngine;
using TMPro;

// Master del HUD. Maneja hotkeys de ventanas y conecta todos los módulos.
public class HUDController : MonoBehaviour
{
    [Header("Panels (toggle con hotkey)")]
    public GameObject inventoryPanel;   // tecla I
    public GameObject characterPanel;  // tecla C

    [Header("Módulos de HUD")]
    public HealthManaDisplay healthMana;
    public ExpBarUI          expBar;
    public SkillBarUI        skillBar;
    public TargetFrameUI     targetFrame;
    public CharacterStatUI   charStats;
    public InventoryUI       inventoryUI;

    [Header("Prompt de interacción (tecla E)")]
    public GameObject          interactPrompt;  // panel que aparece/desaparece
    public TextMeshProUGUI     interactText;    // texto "E - Hablar con Mercader"

    private PlayerCombat      combat;
    private InteractionSystem interactionSys;

    void Start()
    {
        var player      = GameObject.FindGameObjectWithTag("Player");
        combat          = player?.GetComponent<PlayerCombat>();
        interactionSys  = player?.GetComponent<InteractionSystem>();

        CloseAll();
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Update()
    {
        HandleHotkeys();
        SyncTargetFrame();
        SyncInteractPrompt();
    }

    void HandleHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.I)) Toggle(inventoryPanel);
        if (Input.GetKeyDown(KeyCode.C)) Toggle(characterPanel);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryPanel != null && inventoryPanel.activeSelf)  { Toggle(inventoryPanel);  return; }
            if (characterPanel != null && characterPanel.activeSelf)  { Toggle(characterPanel);  return; }
        }
    }

    void SyncTargetFrame()
    {
        if (targetFrame == null || combat == null) return;
        targetFrame.SetTarget(combat.LockedTarget);
    }

    void SyncInteractPrompt()
    {
        if (interactPrompt == null || interactionSys == null) return;
        bool has = interactionSys.HasTarget;
        interactPrompt.SetActive(has);
        if (has && interactText != null)
            interactText.text = $"[E]  {interactionSys.CurrentPrompt}";
    }

    void Toggle(GameObject panel)
    {
        if (panel != null) panel.SetActive(!panel.activeSelf);
    }

    void CloseAll()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (characterPanel != null) characterPanel.SetActive(false);
    }
}
