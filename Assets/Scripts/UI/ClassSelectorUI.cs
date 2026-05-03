using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// Panel de seleccion de clase. Desactivar en la escena del juego.
// Activar en una escena de inicio o como panel sobre la escena principal.
public class ClassSelectorUI : MonoBehaviour
{
    [Header("Referencias")]
    public CharacterClass[] availableClasses;
    public GameObject       panel;
    public Transform        buttonsContainer;
    public GameObject       classButtonPrefab;
    public TextMeshProUGUI  descriptionText;
    public Image            classIconImage;
    public Button           confirmButton;
    public string           gameSceneName = "RPG_Main";

    private CharacterClass  selectedClass;
    private ClassManager    classManager;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        classManager = player?.GetComponent<ClassManager>();

        BuildButtons();

        if (availableClasses.Length > 0)
            SelectClass(availableClasses[0]);

        confirmButton?.onClick.AddListener(ConfirmSelection);
    }

    void BuildButtons()
    {
        if (classButtonPrefab == null || buttonsContainer == null) return;

        foreach (Transform child in buttonsContainer)
            Destroy(child.gameObject);

        foreach (var cls in availableClasses)
        {
            var go  = Instantiate(classButtonPrefab, buttonsContainer);
            var btn = go.GetComponent<Button>();
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            var img = go.GetComponentsInChildren<Image>();

            if (txt != null) txt.text = cls.className;
            if (img.Length > 1 && cls.icon != null) img[1].sprite = cls.icon;

            var captured = cls;
            btn?.onClick.AddListener(() => SelectClass(captured));
        }
    }

    void SelectClass(CharacterClass cls)
    {
        selectedClass = cls;

        if (descriptionText != null)
        {
            descriptionText.text =
                $"<b>{cls.className}</b>\n{cls.description}\n\n" +
                $"FUE: {cls.startSTR}  VIT: {cls.startVIT}  INT: {cls.startINT}  DES: {cls.startDEX}";
        }

        if (classIconImage != null && cls.icon != null)
        {
            classIconImage.sprite = cls.icon;
            classIconImage.color  = cls.classColor;
        }
    }

    void ConfirmSelection()
    {
        if (selectedClass == null) return;

        PlayerPrefs.SetString("SelectedClass", selectedClass.name);
        PlayerPrefs.Save();

        if (classManager != null)
        {
            classManager.ApplyClass(selectedClass);
            panel?.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
