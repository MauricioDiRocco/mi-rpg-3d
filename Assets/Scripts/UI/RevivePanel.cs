using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Panel de muerte: aparece cuando el jugador muere y ofrece dos opciones de revivir.
/// Se agrega al GameObject del panel en el Canvas (empieza desactivado).
/// </summary>
public class RevivePanel : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI expLossText;
    public Button          reviveHereBtn;
    public Button          reviveCityBtn;

    // Refs encontradas en runtime
    PlayerStats      playerStats;
    ExperienceSystem expSystem;
    PlayerController playerCtrl;
    NavMeshAgent     agent;
    Animator         playerAnim;

    static readonly Vector3 CITY_SPAWN = new Vector3(0f, 0.5f, 0f);

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats == null) { enabled = false; return; }

        expSystem  = playerStats.GetComponent<ExperienceSystem>();
        playerCtrl = playerStats.GetComponent<PlayerController>();
        agent      = playerStats.GetComponent<NavMeshAgent>();
        playerAnim = playerStats.GetComponentInChildren<Animator>(true);

        playerStats.OnDeath += OnPlayerDied;

        if (reviveHereBtn) reviveHereBtn.onClick.AddListener(ReviveHere);
        if (reviveCityBtn) reviveCityBtn.onClick.AddListener(ReviveInCity);

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (playerStats != null) playerStats.OnDeath -= OnPlayerDied;
    }

    void OnPlayerDied()
    {
        // Detener al jugador
        if (playerCtrl) playerCtrl.enabled = false;
        if (agent)      { agent.isStopped = true; agent.ResetPath(); }

        // Actualizar texto con % de pérdida
        int lvl = playerStats.level;
        float pct = ExpLossPct(lvl) * 100f;
        if (expLossText)
            expLossText.text = $"Revivir aquí: -{pct:0}% EXP del nivel actual";

        gameObject.SetActive(true);
    }

    void ReviveHere()
    {
        if (expSystem != null)
            expSystem.LoseExp(ExpLossPct(playerStats.level));

        playerStats.currentHP   = playerStats.MaxHP * 0.5f;
        playerStats.currentMana = playerStats.MaxMana;
        Reenable();
    }

    void ReviveInCity()
    {
        if (agent != null) agent.Warp(CITY_SPAWN);
        else               playerStats.transform.position = CITY_SPAWN;

        playerStats.currentHP   = playerStats.MaxHP;
        playerStats.currentMana = playerStats.MaxMana;
        Reenable();
    }

    void Reenable()
    {
        if (playerCtrl) playerCtrl.enabled = true;
        if (agent)      agent.isStopped = false;

        // Volver a animación idle
        if (playerAnim == null)
            playerAnim = playerStats.GetComponentInChildren<Animator>(true);
        if (playerAnim)
        {
            playerAnim.SetFloat("Speed", 0f);
            playerAnim.SetFloat("MoveX", 0f);
            playerAnim.Play("Locomotion");
        }

        gameObject.SetActive(false);
    }

    static float ExpLossPct(int level)
    {
        if (level <= 10) return 0.02f;
        if (level <= 20) return 0.03f;
        if (level <= 30) return 0.04f;
        return 0.05f;
    }
}
