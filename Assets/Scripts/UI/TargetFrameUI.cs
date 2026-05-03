using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Marco del target seleccionado, aparece arriba a la izquierda o centro-arriba.
// Se activa/desactiva según si hay target.
public class TargetFrameUI : MonoBehaviour
{
    public GameObject      panel;        // el panel completo (se activa/desactiva)
    public TextMeshProUGUI targetName;
    public Image           hpBar;        // Fill Horizontal
    public TextMeshProUGUI hpText;

    private EnemyController currentTarget;
    private EnemyStats      currentStats;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (currentTarget != null && currentTarget.IsDead)
            SetTarget(null);
    }

    public void SetTarget(EnemyController target)
    {
        // Desuscribirse del anterior
        if (currentStats != null)
            currentStats.OnHealthChanged -= OnHPChanged;

        currentTarget = target;
        currentStats  = target?.GetComponent<EnemyStats>();

        bool active = target != null;
        if (panel != null) panel.SetActive(active);

        if (!active) return;

        if (targetName != null) targetName.text = target.gameObject.name;

        currentStats.OnHealthChanged += OnHPChanged;
        OnHPChanged(currentStats.currentHP, currentStats.maxHP);
    }

    void OnHPChanged(float current, float max)
    {
        float ratio = max > 0 ? current / max : 0f;
        if (hpBar  != null) hpBar.fillAmount = ratio;
        if (hpText != null) hpText.text      = $"{(int)current} / {(int)max}";
    }

    void OnDestroy()
    {
        if (currentStats != null)
            currentStats.OnHealthChanged -= OnHPChanged;
    }
}
