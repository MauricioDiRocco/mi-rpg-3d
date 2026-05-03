using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Panel de misiones activas (tecla J).
// Muestra lista de quests activas con progreso y botón "Reclamar" cuando están completas.
public class QuestUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject      panel;

    [Header("Lista de quests activas")]
    public Transform       questListContainer;
    public GameObject      questRowPrefab;

    [Header("Detalle")]
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questDescText;
    public TextMeshProUGUI questProgressText;
    public TextMeshProUGUI questRewardText;
    public Button          claimButton;

    private QuestSystem    questSystem;
    private QuestData      selectedQuest;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        questSystem = player?.GetComponent<QuestSystem>();

        if (questSystem != null)
            questSystem.OnQuestUpdated += Refresh;

        panel?.SetActive(false);
        claimButton?.onClick.AddListener(ClaimSelected);
        Refresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            panel?.SetActive(!panel.activeSelf);

        if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            panel.SetActive(false);
    }

    void Refresh()
    {
        if (questListContainer == null || questSystem == null) return;

        foreach (Transform child in questListContainer)
            Destroy(child.gameObject);

        foreach (var prog in questSystem.activeQuests)
        {
            if (questRowPrefab == null) continue;

            var go  = Instantiate(questRowPrefab, questListContainer);
            var btn = go.GetComponent<Button>();
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            var captured = prog;

            string label = prog.completed
                ? $"<color=#88FF88>[!] {prog.quest.questName}</color>"
                : $"{prog.quest.questName} ({prog.currentAmount}/{prog.quest.requiredAmount})";

            if (txt != null) txt.text = label;
            btn?.onClick.AddListener(() => ShowDetail(captured.quest));
        }

        if (selectedQuest != null)
            ShowDetail(selectedQuest);
    }

    void ShowDetail(QuestData quest)
    {
        selectedQuest = quest;
        var prog = questSystem.GetProgress(quest);

        if (questNameText   != null) questNameText.text   = quest.questName;
        if (questDescText   != null) questDescText.text   = quest.description;
        if (questRewardText != null) questRewardText.text = $"Recompensa: {quest.rewardGold} Yang + {quest.rewardExp} EXP";

        if (questProgressText != null && prog != null)
            questProgressText.text = $"Progreso: {prog.currentAmount} / {quest.requiredAmount}";

        bool canClaim = prog != null && prog.completed && !prog.rewardClaimed;
        claimButton?.gameObject.SetActive(canClaim);
    }

    void ClaimSelected()
    {
        if (selectedQuest == null || questSystem == null) return;
        questSystem.ClaimReward(selectedQuest);
        selectedQuest = null;
        Refresh();
    }

    void OnDestroy()
    {
        if (questSystem != null)
            questSystem.OnQuestUpdated -= Refresh;
    }
}
