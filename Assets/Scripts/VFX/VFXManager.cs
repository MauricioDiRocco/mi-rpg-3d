using System.Collections.Generic;
using UnityEngine;

// Singleton. Spawnea efectos de partículas usando un pool.
// Colocar en la escena y asignar los prefabs de VFX.
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Prefabs de VFX (Particle Systems)")]
    public GameObject vfxHit;
    public GameObject vfxCritical;
    public GameObject vfxHeal;
    public GameObject vfxLevelUp;
    public GameObject vfxPickup;
    public GameObject vfxPortal;
    public GameObject vfxEnemyDeath;
    public GameObject vfxBossPhaseChange;

    [Header("Pool size")]
    public int poolSize = 20;

    private Dictionary<GameObject, Queue<GameObject>> pool = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public static void SpawnHit(Vector3 pos)          => Spawn(Instance?.vfxHit,             pos);
    public static void SpawnCritical(Vector3 pos)     => Spawn(Instance?.vfxCritical,        pos);
    public static void SpawnHeal(Vector3 pos)         => Spawn(Instance?.vfxHeal,            pos);
    public static void SpawnLevelUp(Vector3 pos)      => Spawn(Instance?.vfxLevelUp,         pos);
    public static void SpawnPickup(Vector3 pos)       => Spawn(Instance?.vfxPickup,          pos);
    public static void SpawnPortal(Vector3 pos)       => Spawn(Instance?.vfxPortal,          pos);
    public static void SpawnEnemyDeath(Vector3 pos)   => Spawn(Instance?.vfxEnemyDeath,      pos);
    public static void SpawnBossPhase(Vector3 pos)    => Spawn(Instance?.vfxBossPhaseChange, pos);

    static void Spawn(GameObject prefab, Vector3 pos)
    {
        if (prefab == null || Instance == null) return;

        GameObject go;
        if (Instance.pool.TryGetValue(prefab, out var queue) && queue.Count > 0)
        {
            go = queue.Dequeue();
            go.transform.position = pos;
            go.SetActive(true);
        }
        else
        {
            go = Instantiate(prefab, pos, Quaternion.identity);
        }

        var ps = go.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            Instance.StartCoroutine(Instance.ReturnToPool(prefab, go, ps.main.duration + ps.main.startLifetime.constantMax));
        }
        else
        {
            Destroy(go, 3f);
        }
    }

    System.Collections.IEnumerator ReturnToPool(GameObject prefab, GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);

        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        if (pool[prefab].Count < poolSize)
            pool[prefab].Enqueue(instance);
        else
            Destroy(instance);
    }
}
