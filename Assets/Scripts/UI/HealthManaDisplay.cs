using UnityEngine;
using UnityEngine.UI;
using TMPro;

// HUD principal: orbs circulares estilo Metin2 (HP rojo izq, Mana azul der).
// Usar Image con Fill Method = Radial360 para las gemas.
public class HealthManaDisplay : MonoBehaviour
{
    [Header("HP Orb (izquierda)")]
    public Image              hpFill;    // ImageType=Filled, FillMethod=Radial360, FillOrigin=Top
    public TextMeshProUGUI    hpText;

    [Header("Mana Orb (derecha)")]
    public Image              manaFill;
    public TextMeshProUGUI    manaText;

    private PlayerStats stats;

    void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        if (stats != null)
            stats.OnStatsChanged += Refresh;
    }

    void Update()
    {
        if (stats == null) return;

        if (hpFill   != null) hpFill.fillAmount   = stats.currentHP   / stats.MaxHP;
        if (manaFill != null) manaFill.fillAmount  = stats.currentMana / stats.MaxMana;
        if (hpText   != null) hpText.text          = $"{(int)stats.currentHP}/{(int)stats.MaxHP}";
        if (manaText != null) manaText.text        = $"{(int)stats.currentMana}/{(int)stats.MaxMana}";
    }

    void Refresh() { } // fuerza re-render del max en el mismo frame

    void OnDestroy()
    {
        if (stats != null) stats.OnStatsChanged -= Refresh;
    }
}
