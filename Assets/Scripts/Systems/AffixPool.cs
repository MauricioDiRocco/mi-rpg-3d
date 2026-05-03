using UnityEngine;

// Singleton que carga todos los AffixData del proyecto.
// Colocar en un GameObject de la escena o en el Player.
public class AffixPool : MonoBehaviour
{
    public static AffixPool Instance { get; private set; }

    [Header("Pool de affixes (arrastrar todos los AffixData acá)")]
    public AffixData[] affixes;

    [Header("Magic Find del player (1.0 = normal)")]
    public float baseMagicFind = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public static AffixData[] GetPool() => Instance != null ? Instance.affixes : null;

    public static float GetMagicFind()
    {
        if (Instance == null) return 1f;
        var player = GameObject.FindGameObjectWithTag("Player");
        // TODO: sumar MagicFind del equipo cuando se implemente en ItemData
        return Instance.baseMagicFind;
    }

    public static ItemData Generate(ItemData baseItem, int itemLevel = 1)
    {
        if (Instance == null) return baseItem;
        float mf   = GetMagicFind();
        var rarity = ItemGenerator.RollRarity(mf);
        return ItemGenerator.GenerateItem(baseItem, rarity, GetPool(), mf, itemLevel);
    }
}
