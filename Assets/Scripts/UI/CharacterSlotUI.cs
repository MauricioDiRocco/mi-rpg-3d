using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    [Header("Visuals")]
    public Image             background;
    public Image             selectedBorder;
    public TextMeshProUGUI   classText;
    public TextMeshProUGUI   nameText;
    public TextMeshProUGUI   levelText;
    public TextMeshProUGUI   emptyText;
    public Button            button;

    static readonly Color BG_NORMAL   = new Color(0.07f, 0.05f, 0.02f, 0.97f);
    static readonly Color BG_SELECTED = new Color(0.30f, 0.20f, 0.04f, 1f);

    public void Setup(CharacterData data, int index, Action<int> onClick)
    {
        bool filled = data != null;

        classText.text = filled ? data.race.ToUpper() : "";
        nameText.text  = filled ? data.characterName  : "";
        levelText.text = filled ? $"Nivel {data.level}" : "";

        classText.gameObject.SetActive(filled);
        nameText.gameObject.SetActive(filled);
        levelText.gameObject.SetActive(filled);
        emptyText.gameObject.SetActive(!filled);

        SetSelected(false);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(index));
    }

    public void SetSelected(bool selected)
    {
        if (background     != null) background.color = selected ? BG_SELECTED : BG_NORMAL;
        if (selectedBorder != null) selectedBorder.gameObject.SetActive(selected);
    }
}
