using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharSelectUI : MonoBehaviour
{
    [Header("Slots (3)")]
    public List<CharacterSlotUI> slots;

    [Header("Botones principales")]
    public Button              playBtn;
    public Button              deleteBtn;
    public Button              backBtn;

    [Header("Panel Crear Personaje")]
    public GameObject          createPanel;
    public TMP_InputField      createNameField;
    public Button              createConfirmBtn;
    public Button              createCancelBtn;
    public TextMeshProUGUI     createStatusText;

    [Header("Texto de cuenta")]
    public TextMeshProUGUI     accountLabel;

    int selectedSlot = -1;

    void Start()
    {
        EnsureAccountManager();

        if (accountLabel != null && AccountManager.Instance.CurrentAccount != null)
            accountLabel.text = $"Cuenta: {AccountManager.Instance.CurrentAccount.username}";

        playBtn.onClick.AddListener(OnPlay);
        deleteBtn.onClick.AddListener(OnDelete);
        backBtn.onClick.AddListener(OnBack);
        if (createConfirmBtn != null) createConfirmBtn.onClick.AddListener(OnCreateConfirm);
        if (createCancelBtn  != null) createCancelBtn.onClick.AddListener(() => { if (createPanel) createPanel.SetActive(false); });
        if (createPanel      != null) createPanel.SetActive(false);

        RefreshSlots();
    }

    void RefreshSlots()
    {
        var chars = AccountManager.Instance?.CurrentAccount?.characters ?? new List<CharacterData>();
        for (int i = 0; i < slots.Count; i++)
        {
            CharacterData data = i < chars.Count ? chars[i] : null;
            slots[i].Setup(data, i, OnSlotClicked);
        }
        selectedSlot = -1;
        playBtn.gameObject.SetActive(false);
        deleteBtn.gameObject.SetActive(false);
    }

    void OnSlotClicked(int index)
    {
        var chars = AccountManager.Instance?.CurrentAccount?.characters ?? new List<CharacterData>();

        selectedSlot = index;
        foreach (var s in slots) s.SetSelected(false);
        slots[index].SetSelected(true);

        if (index < chars.Count)
        {
            if (createPanel != null) createPanel.SetActive(false);
            playBtn.gameObject.SetActive(true);
            deleteBtn.gameObject.SetActive(true);
        }
        else
        {
            // Slot vacío → abrir panel de creación
            playBtn.gameObject.SetActive(false);
            deleteBtn.gameObject.SetActive(false);
            if (createNameField  != null) createNameField.text  = "";
            if (createStatusText != null) createStatusText.text = "";
            if (createPanel      != null) createPanel.SetActive(true);
        }
    }

    void OnCreateConfirm()
    {
        string cname = createNameField.text.Trim();
        if (cname.Length < 3)
        {
            createStatusText.text = "El nombre necesita al menos 3 letras.";
            return;
        }

        var ch = new CharacterData
        {
            characterName = cname,
            race          = "Guerrero",
            level         = 1,
            strength      = 12,
            vitality      = 10,
            intelligence  = 5,
            dexterity     = 8,
            statPoints    = 0,
        };
        AccountManager.Instance.AddCharacter(ch);
        createPanel.SetActive(false);
        RefreshSlots();
    }

    void OnPlay()
    {
        var chars = AccountManager.Instance?.CurrentAccount?.characters;
        if (chars == null || selectedSlot < 0 || selectedSlot >= chars.Count) return;
        AccountManager.Instance.SelectCharacter(chars[selectedSlot]);
        SceneManager.LoadScene("RPG_Main");
    }

    void OnDelete()
    {
        var chars = AccountManager.Instance?.CurrentAccount?.characters;
        if (chars == null || selectedSlot < 0 || selectedSlot >= chars.Count) return;
        AccountManager.Instance.DeleteCharacter(selectedSlot);
        RefreshSlots();
    }

    void OnBack()
    {
        AccountManager.Instance?.Logout();
        SceneManager.LoadScene("LoginScene");
    }

    static void EnsureAccountManager()
    {
        if (AccountManager.Instance == null)
            new GameObject("AccountManager").AddComponent<AccountManager>();
    }
}
