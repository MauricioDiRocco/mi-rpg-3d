using UnityEngine;

// Modifier que convierte un enemy normal en Elite.
// Agregar junto con EnemyStats + EnemyController.
// Escala sus stats y le da un prefijo de nombre.
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(LootDropper))]
public class EliteEnemy : MonoBehaviour
{
    [Header("Multiplicadores de stats")]
    public float hpMultiplier       = 3f;
    public float damageMultiplier   = 1.8f;
    public float defenseMultiplier  = 2f;
    public float expMultiplier      = 4f;
    public float magicFindBonus     = 1.5f;

    [Header("Identificacion")]
    public string prefixName = "Elite";
    public Color  nameColor  = Color.yellow;

    private EnemyStats stats;
    private LootDropper dropper;

    void Awake()
    {
        stats   = GetComponent<EnemyStats>();
        dropper = GetComponent<LootDropper>();

        stats.maxHP      *= hpMultiplier;
        stats.damage     *= damageMultiplier;
        stats.defense    *= defenseMultiplier;
        stats.expReward  = (long)(stats.expReward * expMultiplier);

        gameObject.name = $"[{prefixName}] {gameObject.name}";

        // Aumentar magic find del loot table
        if (dropper != null)
            dropper.spreadRadius += 0.5f;
    }

    void Start()
    {
        // Visual: teñir el renderer de amarillo/dorado
        foreach (var rend in GetComponentsInChildren<Renderer>())
        {
            if (rend.material.HasProperty("_BaseColor"))
                rend.material.SetColor("_BaseColor", nameColor);
        }
    }
}
