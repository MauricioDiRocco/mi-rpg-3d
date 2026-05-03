using UnityEngine;
using System.Collections.Generic;

public enum QuestType { KillEnemies, CollectItems, ReachZone, TalkToNPC }

[CreateAssetMenu(fileName = "NewQuest", menuName = "RPG/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Identificacion")]
    public string    questName    = "Mision";
    public string    description  = "";

    [Header("Tipo y objetivo")]
    public QuestType questType    = QuestType.KillEnemies;
    public string    targetTag    = "Enemy";       // tag del enemy a matar / nombre de zona
    public ItemData  targetItem;                   // item a recolectar (si CollectItems)
    public int       requiredAmount = 5;

    [Header("Recompensa")]
    public int       rewardGold   = 100;
    public long      rewardExp    = 200;
    public ItemData  rewardItem;                   // item extra de recompensa (opcional)

    [Header("Requerimientos")]
    public int       requiredLevel = 1;
    public QuestData requiredQuest;                // quest que debe estar completada antes
}
