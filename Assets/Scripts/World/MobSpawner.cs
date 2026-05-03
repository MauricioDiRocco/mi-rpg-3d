using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Spawner de mobs para zonas. Mantiene una cantidad constante de enemigos vivos.
// Respawna después del cooldown cuando los mobs mueren.
public class MobSpawner : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject[] mobPrefabs;
    public int          maxAlive       = 8;
    public float        spawnRadius    = 15f;
    public float        respawnDelay   = 20f;

    [Header("Escalado por nivel de zona")]
    public int   zoneLevel           = 1;
    [Range(0f, 2f)]
    public float statMultiplier      = 1f;

    private int   aliveCount;
    private bool  active = true;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (active)
        {
            while (aliveCount < maxAlive)
            {
                SpawnOne();
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(respawnDelay);
        }
    }

    void SpawnOne()
    {
        if (mobPrefabs.Length == 0) return;

        var prefab = mobPrefabs[Random.Range(0, mobPrefabs.Length)];
        Vector3 pos = GetSpawnPosition();

        var go = Instantiate(prefab, pos, Quaternion.identity);
        aliveCount++;

        // Escalar stats por zona
        var eStats = go.GetComponent<EnemyStats>();
        if (eStats != null)
        {
            if (statMultiplier != 1f)
            {
                eStats.maxHP     *= statMultiplier;
                eStats.damage    *= statMultiplier;
                eStats.defense   *= statMultiplier;
                eStats.expReward  = (long)(eStats.expReward * statMultiplier);
            }
            // Nivel del item: zona ± variación pequeña
            eStats.itemLevel = Mathf.Max(1, zoneLevel + Random.Range(-1, 3));
        }

        // Decrementar cuando muere
        var deathStats = go.GetComponent<EnemyStats>();
        if (deathStats != null)
            deathStats.OnDeath += () => aliveCount = Mathf.Max(0, aliveCount - 1);
    }

    Vector3 GetSpawnPosition()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            var offset = Random.insideUnitCircle * spawnRadius;
            var candidate = transform.position + new Vector3(offset.x, 0f, offset.y);

            if (NavMesh.SamplePosition(candidate, out var hit, 3f, NavMesh.AllAreas))
                return hit.position;
        }
        return transform.position;
    }

    void OnDisable()  => active = false;
    void OnEnable()   => active = true;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = new Color(1f, 0.5f, 0f, 0.2f);
        UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, spawnRadius);
        UnityEditor.Handles.color = new Color(1f, 0.5f, 0f, 0.8f);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, spawnRadius);
    }
#endif
}
