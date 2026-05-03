using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Barra de EXP fina en el fondo de la pantalla, estilo Metin2.
// Image: Fill Method=Horizontal, color dorado/amarillo.
public class ExpBarUI : MonoBehaviour
{
    public Image              expFill;
    public TextMeshProUGUI    expText;
    public TextMeshProUGUI    levelText;

    private PlayerStats      stats;
    private ExperienceSystem expSys;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        stats      = player?.GetComponent<PlayerStats>();
        expSys     = player?.GetComponent<ExperienceSystem>();

        if (expSys != null)
        {
            expSys.OnExpChanged += Refresh;
            expSys.OnLevelUp    += OnLevelUp;
        }
        Refresh();
    }

    void Refresh()
    {
        if (expSys == null || stats == null) return;

        if (expFill   != null) expFill.fillAmount = expSys.ExpPercent;
        if (expText   != null) expText.text       = $"{expSys.currentExp:N0} / {expSys.ExpForNextLevel:N0}";
        if (levelText != null) levelText.text     = $"Lv. {stats.level}";
    }

    void OnLevelUp(int newLevel) => Refresh();

    void OnDestroy()
    {
        if (expSys == null) return;
        expSys.OnExpChanged -= Refresh;
        expSys.OnLevelUp    -= OnLevelUp;
    }
}
