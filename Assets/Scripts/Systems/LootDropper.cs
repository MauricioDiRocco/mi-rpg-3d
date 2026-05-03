using UnityEngine;

// Agregar al prefab del enemigo junto con EnemyController.
[RequireComponent(typeof(EnemyStats))]
public class LootDropper : MonoBehaviour
{
    public LootTable  lootTable;
    public GameObject droppedItemPrefab;

    [Range(0f, 3f)]
    public float spreadRadius = 1.2f;

    void Awake()
    {
        GetComponent<EnemyStats>().OnDeath += DropLoot;
    }

    // Llamar manualmente desde cofres, quests, etc.
    public void Drop() => DropLoot();

    void DropLoot()
    {
        if (lootTable == null || droppedItemPrefab == null) return;

        // Items
        foreach (var (item, qty) in lootTable.RollLoot())
            SpawnDrop(item, qty);

        // Gold
        int gold = lootTable.RollGold();
        if (gold > 0)
            SpawnGold(gold);
    }

    void SpawnDrop(ItemData item, int qty)
    {
        int lvl = GetComponent<EnemyStats>()?.itemLevel ?? 1;
        var finalItem = item.type != ItemType.Potion && item.type != ItemType.Gold
            ? AffixPool.Generate(item, lvl)
            : item;

        var go   = Instantiate(droppedItemPrefab, RandomPos(), Quaternion.identity);
        var drop = go.GetComponent<DroppedItem>();
        drop?.Initialize(finalItem, qty);
    }

    void SpawnGold(int amount)
    {
        var go   = Instantiate(droppedItemPrefab, RandomPos(), Quaternion.identity);
        var drop = go.GetComponent<DroppedItem>();
        drop?.InitializeGold(amount);
    }

    Vector3 RandomPos()
    {
        var offset = Random.insideUnitCircle * spreadRadius;
        return transform.position + new Vector3(offset.x, 0.15f, offset.y);
    }
}
