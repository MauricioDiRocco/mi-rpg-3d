using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Panel de configuración (Esc → Settings): volumen BGM/SFX, resolución, calidad.
// Tecla Escape abre/cierra. Guardar en PlayerPrefs.
public class SettingsUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Audio")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI bgmLabel;
    public TextMeshProUGUI sfxLabel;

    [Header("Gameplay")]
    public Button saveBtn;
    public Button loadBtn;
    public Button deleteBtn;
    public Button quitBtn;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    private SaveSystem saveSystem;

    void Start()
    {
        panel?.SetActive(false);

        var player = GameObject.FindGameObjectWithTag("Player");
        saveSystem = player?.GetComponent<SaveSystem>();

        float bgm = PlayerPrefs.GetFloat("BGMVolume", 0.4f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        if (bgmSlider != null) { bgmSlider.value = bgm; bgmSlider.onValueChanged.AddListener(OnBGMChanged); }
        if (sfxSlider != null) { sfxSlider.value = sfx; sfxSlider.onValueChanged.AddListener(OnSFXChanged); }
        if (bgmLabel  != null) bgmLabel.text = $"Música: {bgm:P0}";
        if (sfxLabel  != null) sfxLabel.text = $"SFX: {sfx:P0}";

        saveBtn?.onClick.AddListener(() => { saveSystem?.Save();   SetFeedback("Partida guardada (F5)"); });
        loadBtn?.onClick.AddListener(() => { saveSystem?.Load();   SetFeedback("Partida cargada (F9)"); });
        deleteBtn?.onClick.AddListener(() => { saveSystem?.DeleteSave(); SetFeedback("Partida eliminada"); });
        quitBtn?.onClick.AddListener(() => Application.Quit());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            panel?.SetActive(!panel.activeSelf);
    }

    void OnBGMChanged(float val)
    {
        AudioManager.Instance?.SetBGMVolume(val);
        PlayerPrefs.SetFloat("BGMVolume", val);
        if (bgmLabel != null) bgmLabel.text = $"Música: {val:P0}";
    }

    void OnSFXChanged(float val)
    {
        AudioManager.Instance?.SetSFXVolume(val);
        PlayerPrefs.SetFloat("SFXVolume", val);
        if (sfxLabel != null) sfxLabel.text = $"SFX: {val:P0}";
    }

    void SetFeedback(string msg)
    {
        if (feedbackText != null) feedbackText.text = msg;
        CancelInvoke(nameof(ClearFeedback));
        Invoke(nameof(ClearFeedback), 2f);
    }

    void ClearFeedback()
    {
        if (feedbackText != null) feedbackText.text = "";
    }
}
