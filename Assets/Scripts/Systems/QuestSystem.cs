using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestProgress
{
    public QuestData quest;
    public int       currentAmount;
    public bool      completed;
    public bool      rewardClaimed;
}

public class QuestSystem : MonoBehaviour
{
    public List<QuestProgress> activeQuests  = new();
    public List<QuestData>     completedList = new();

    public event Action OnQuestUpdated;
    public event Action<QuestData> OnQuestCompleted;

    private PlayerStats      stats;
    private ExperienceSystem expSys;
    private Inventory        inventory;

    void Awake()
    {
        stats     = GetComponent<PlayerStats>();
        expSys    = GetComponent<ExperienceSystem>();
        inventory = GetComponent<Inventory>();
    }

    void OnEnable()
    {
        // Escuchar muertes de enemigos (via broadcast estático)
        EnemyStats.OnAnyEnemyDied += HandleEnemyKill;
    }

    void OnDisable()
    {
        EnemyStats.OnAnyEnemyDied -= HandleEnemyKill;
    }

    public bool AcceptQuest(QuestData quest)
    {
        if (stats.level < quest.requiredLevel)
        {
            Debug.Log($"[Quest] Nivel insuficiente para {quest.questName} (req. {quest.requiredLevel})");
            return false;
        }
        if (completedList.Contains(quest))
        {
            Debug.Log($"[Quest] {quest.questName} ya completada");
            return false;
        }
        if (activeQuests.Exists(q => q.quest == quest))
        {
            Debug.Log($"[Quest] {quest.questName} ya aceptada");
            return false;
        }
        if (quest.requiredQuest != null && !completedList.Contains(quest.requiredQuest))
        {
            Debug.Log($"[Quest] Primero completá: {quest.requiredQuest.questName}");
            return false;
        }

        activeQuests.Add(new QuestProgress { quest = quest });
        Debug.Log($"[Quest] Aceptada: {quest.questName}");
        OnQuestUpdated?.Invoke();
        return true;
    }

    void HandleEnemyKill(string enemyTag)
    {
        foreach (var prog in activeQuests)
        {
            if (prog.completed) continue;
            if (prog.quest.questType != QuestType.KillEnemies) continue;
            if (prog.quest.targetTag != enemyTag && prog.quest.targetTag != "Enemy") continue;

            prog.currentAmount++;
            if (prog.currentAmount >= prog.quest.requiredAmount)
                CompleteQuest(prog);
            else
                OnQuestUpdated?.Invoke();
        }
    }

    public void NotifyItemCollected(ItemData item)
    {
        foreach (var prog in activeQuests)
        {
            if (prog.completed) continue;
            if (prog.quest.questType != QuestType.CollectItems) continue;
            if (prog.quest.targetItem != item) continue;

            prog.currentAmount++;
            if (prog.currentAmount >= prog.quest.requiredAmount)
                CompleteQuest(prog);
            else
                OnQuestUpdated?.Invoke();
        }
    }

    void CompleteQuest(QuestProgress prog)
    {
        prog.completed = true;
        Debug.Log($"[Quest] ¡Completada! {prog.quest.questName}");
        OnQuestCompleted?.Invoke(prog.quest);
        OnQuestUpdated?.Invoke();
    }

    public void ClaimReward(QuestData quest)
    {
        var prog = activeQuests.Find(p => p.quest == quest && p.completed && !p.rewardClaimed);
        if (prog == null) return;

        inventory.AddGold(quest.rewardGold);
        expSys.GainExp(quest.rewardExp);

        if (quest.rewardItem != null)
            inventory.AddItem(quest.rewardItem);

        prog.rewardClaimed = true;
        completedList.Add(quest);
        activeQuests.Remove(prog);

        Debug.Log($"[Quest] Recompensa reclamada: {quest.rewardGold} Yang + {quest.rewardExp} EXP");
        OnQuestUpdated?.Invoke();
    }

    public QuestProgress GetProgress(QuestData quest) =>
        activeQuests.Find(p => p.quest == quest);

    public bool IsCompleted(QuestData quest) => completedList.Contains(quest);
}
