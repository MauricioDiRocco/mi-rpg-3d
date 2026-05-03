using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Ventana de stats (tecla C), estilo Metin2.
// Panel izquierdo: STR/VIT/INT/DEX con botones +
// Panel derecho: stats derivados (HP, mana, daño, def, vel)
public class CharacterStatUI : MonoBehaviour
{
    [Header("Info del personaje")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI statPointsText;

    [Header("Stats base + botones +")]
    public TextMeshProUGUI strText;
    public TextMeshProUGUI vitText;
    public TextMeshProUGUI intText;
    public TextMeshProUGUI dexText;
    public Button          strPlusBtn;
    public Button          vitPlusBtn;
    public Button          intPlusBtn;
    public Button          dexPlusBtn;

    [Header("Stats derivados")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI physDmgText;
    public TextMeshProUGUI magicDmgText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI attackSpeedText;

    private PlayerStats      stats;
    private EquipmentManager equipment;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        stats      = player?.GetComponent<PlayerStats>();
        equipment  = player?.GetComponent<EquipmentManager>();

        if (stats != null)     stats.OnStatsChanged          += Refresh;
        if (equipment != null) equipment.OnEquipmentChanged  += Refresh;

        strPlusBtn?.onClick.AddListener(() => stats?.AddStatPoint("STR"));
        vitPlusBtn?.onClick.AddListener(() => stats?.AddStatPoint("VIT"));
        intPlusBtn?.onClick.AddListener(() => stats?.AddStatPoint("INT"));
        dexPlusBtn?.onClick.AddListener(() => stats?.AddStatPoint("DEX"));

        Refresh();
    }

    void OnEnable() => Refresh();

    void OnDestroy()
    {
        if (stats     != null) stats.OnStatsChanged           -= Refresh;
        if (equipment != null) equipment.OnEquipmentChanged   -= Refresh;
    }

    void Refresh()
    {
        if (stats == null) return;

        int bSTR = equipment?.BonusSTR ?? 0;
        int bVIT = equipment?.BonusVIT ?? 0;
        int bINT = equipment?.BonusINT ?? 0;
        int bDEX = equipment?.BonusDEX ?? 0;

        if (levelText)     levelText.text     = $"Nivel  {stats.level}";
        if (statPointsText)statPointsText.text = $"Puntos: {stats.statPoints}";

        if (strText) strText.text = StatLine("FUE", stats.strength,     bSTR);
        if (vitText) vitText.text = StatLine("VIT", stats.vitality,     bVIT);
        if (intText) intText.text = StatLine("INT", stats.intelligence, bINT);
        if (dexText) dexText.text = StatLine("DES", stats.dexterity,    bDEX);

        float eHP  = equipment?.BonusHP          ?? 0;
        float eMana= equipment?.BonusMana        ?? 0;
        float ePhy = equipment?.BonusPhysDamage  ?? 0;
        float eMag = equipment?.BonusMagicDamage ?? 0;
        float eDef = equipment?.BonusDefense     ?? 0;

        if (hpText)          hpText.text          = $"HP:          {(int)(stats.MaxHP + eHP)}";
        if (manaText)        manaText.text        = $"Mana:        {(int)(stats.MaxMana + eMana)}";
        if (physDmgText)     physDmgText.text     = $"Daño Físico: {stats.PhysDamage + ePhy:F0}";
        if (magicDmgText)    magicDmgText.text    = $"Daño Mágico: {stats.MagicDamage + eMag:F0}";
        if (defText)         defText.text         = $"Defensa:     {stats.Defense + eDef:F0}";
        if (moveSpeedText)   moveSpeedText.text   = $"Vel. Mov:    {stats.MoveSpeed:F2}";
        if (attackSpeedText) attackSpeedText.text = $"Vel. Atq:    {stats.AttackSpeed:F2}/s";

        bool hasPoints = stats.statPoints > 0;
        strPlusBtn?.gameObject.SetActive(hasPoints);
        vitPlusBtn?.gameObject.SetActive(hasPoints);
        intPlusBtn?.gameObject.SetActive(hasPoints);
        dexPlusBtn?.gameObject.SetActive(hasPoints);
    }

    static string StatLine(string label, int baseVal, int bonus)
    {
        return bonus > 0
            ? $"{label}:  {baseVal} <color=#88FFAA>(+{bonus})</color>"
            : $"{label}:  {baseVal}";
    }
}
