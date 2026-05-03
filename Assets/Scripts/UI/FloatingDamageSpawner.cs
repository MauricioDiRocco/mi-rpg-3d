using UnityEngine;

// Singleton. Poner en un GameObject vacío en la escena o en el Player.
// Asignar el prefab de FloatingDamage en el Inspector.
public class FloatingDamageSpawner : MonoBehaviour
{
    public static FloatingDamageSpawner Instance { get; private set; }

    public GameObject floatingDamagePrefab;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public static void Spawn(Vector3 worldPos, float amount, DamageType type)
    {
        if (Instance == null || Instance.floatingDamagePrefab == null) return;
        var go = Instantiate(
            Instance.floatingDamagePrefab,
            worldPos + Vector3.up * 1.8f,
            Quaternion.identity
        );
        go.GetComponent<FloatingDamage>()?.Initialize(amount, type);
    }
}
