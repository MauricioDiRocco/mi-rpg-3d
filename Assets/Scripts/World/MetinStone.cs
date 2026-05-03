using System.Collections;
using UnityEngine;

// Metin Stone: objeto destruible que genera mobs alrededor y da loot mejorado al morir.
// Colocar en la escena como objeto estático grande con Collider.
public class MetinStone : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP            = 500f;
    public int   defenseReduction = 0;   // las piedras no tienen defensa por defecto

    [Header("Spawning")]
    public GameObject[] mobPrefabs;
    public int          maxActiveMobs   = 5;
    public float        spawnRadius     = 6f;
    public float        spawnInterval   = 8f;

    [Header("Recompensa")]
    public LootTable lootTable;
    public GameObject droppedItemPrefab;
    public long       bonusExp        = 200;
    public int        bonusGoldMin    = 50;
    public int        bonusGoldMax    = 200;
    public float      lootMagicFind   = 2f; // 2× magic find en drops

    [Header("Visual")]
    public Renderer stoneRenderer;
    public Color    damagedColor      = new Color(1f, 0.3f, 0.1f);

    private float         currentHP;
    private bool          isDead;
    private int           activeMobCount;

    public event System.Action<float, float> OnHealthChanged;

    void Awake()
    {
        currentHP = maxHP;
    }

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP = Mathf.Clamp(currentHP - amount, 0f, maxHP);
        FloatingDamageSpawner.Spawn(transform.position + Vector3.up * 2f, amount, DamageType.Physical);
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (stoneRenderer != null)
        {
            float t = 1f - (currentHP / maxHP);
            stoneRenderer.material.color = Color.Lerp(Color.white, damagedColor, t);
        }

        if (currentHP <= 0f) Die();
    }

    void OnMouseDown()
    {
        // El player puede hacer click en la piedra para atacarla (usa sistema de raycast del PlayerController)
        // La lógica de ataque viene de PlayerCombat a través de EnemyController.TakeDamage
        // Esta piedra necesita ser atacada manualmente por el diseñador con TakeDamage o desde PlayerController override
    }

    void Die()
    {
        isDead = true;

        var player = GameObject.FindGameObjectWithTag("Player");
        player?.GetComponent<ExperienceSystem>()?.GainExp(bonusExp);

        if (droppedItemPrefab != null)
        {
            int gold = Random.Range(bonusGoldMin, bonusGoldMax + 1);
            if (gold > 0)
            {
                var go = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
                go.GetComponent<DroppedItem>()?.InitializeGold(gold);
            }

            if (lootTable != null)
            {
                foreach (var (item, qty) in lootTable.RollLoot(lootMagicFind))
                {
                    var offset = Random.insideUnitCircle * 2f;
                    var pos = transform.position + new Vector3(offset.x, 0.2f, offset.y);
                    var go = Instantiate(droppedItemPrefab, pos, Quaternion.identity);
                    go.GetComponent<DroppedItem>()?.Initialize(item, qty);
                }
            }
        }

        Debug.Log($"[MetinStone] {gameObject.name} destruida! +{bonusExp} EXP");
        Destroy(gameObject, 0.5f);
    }

    IEnumerator SpawnLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (!isDead && activeMobCount < maxActiveMobs && mobPrefabs.Length > 0)
                SpawnMob();
        }
    }

    void SpawnMob()
    {
        var prefab = mobPrefabs[Random.Range(0, mobPrefabs.Length)];
        var offset = Random.insideUnitCircle.normalized * spawnRadius;
        var pos = transform.position + new Vector3(offset.x, 0f, offset.y);

        var go = Instantiate(prefab, pos, Quaternion.identity);
        activeMobCount++;

        // Cuando el mob muere, decrementamos el contador
        var enemyStats = go.GetComponent<EnemyStats>();
        if (enemyStats != null)
            enemyStats.OnDeath += () => activeMobCount = Mathf.Max(0, activeMobCount - 1);
    }
}
