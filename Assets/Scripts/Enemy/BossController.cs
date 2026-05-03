using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Boss con múltiples fases. Extiende comportamiento de EnemyController.
// Requiere: NavMeshAgent, EnemyStats, LootDropper en el mismo GO.
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyStats))]
public class BossController : MonoBehaviour
{
    [Header("IA")]
    public float detectionRange   = 20f;
    public float meleeRange       = 2.5f;
    public float chargeRange      = 12f;

    [Header("Habilidades del Boss")]
    public float chargeCooldown   = 8f;
    public float chargeSpeed      = 12f;
    public float aoeAttackCooldown = 5f;
    public float aoeRadius         = 4f;
    public float aoeDamageMultiplier = 1.5f;

    [Header("Fases (% HP)")]
    [Tooltip("Al llegar a este % de HP activa Fase 2")]
    public float phase2Threshold  = 0.5f;
    [Tooltip("Al llegar a este % de HP activa Fase 3")]
    public float phase3Threshold  = 0.25f;

    [Header("Anuncio de fases")]
    public string phase2Message   = "¡El Boss entra en cólera!";
    public string phase3Message   = "¡Poder FINAL desbloqueado!";

    private NavMeshAgent   agent;
    private EnemyStats     stats;
    private Transform      player;
    private PlayerStats    playerStats;
    private ExperienceSystem playerExp;

    private int   currentPhase = 1;
    private bool  isDead;
    private float chargeTimer;
    private float aoeTimer;
    private float attackTimer;

    public bool IsDead => isDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<EnemyStats>();
        stats.OnDeath += HandleDeath;
        stats.OnHealthChanged += CheckPhaseTransition;
    }

    void Start()
    {
        var playerGO  = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player      = playerGO.transform;
            playerStats = playerGO.GetComponent<PlayerStats>();
            playerExp   = playerGO.GetComponent<ExperienceSystem>();
        }
        agent.speed = stats.moveSpeed;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        chargeTimer -= Time.deltaTime;
        aoeTimer    -= Time.deltaTime;

        if (dist <= detectionRange)
        {
            FaceTarget(player);

            if (dist <= meleeRange)
            {
                agent.isStopped = true;
                MeleeAttack();
            }
            else
            {
                agent.isStopped = false;
                agent.speed = stats.moveSpeed;
                agent.SetDestination(player.position);

                if (chargeTimer <= 0f && dist <= chargeRange)
                    StartCoroutine(ChargeAttack());

                if (aoeTimer <= 0f && dist <= aoeRadius * 2f)
                    AoEAttack();
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    void MeleeAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;

        float dmg = stats.damage * (currentPhase >= 2 ? 1.4f : 1f);
        playerStats?.TakeDamage(dmg);
        attackTimer = 1f / stats.attackSpeed;
    }

    IEnumerator ChargeAttack()
    {
        chargeTimer = chargeCooldown * (currentPhase >= 3 ? 0.6f : 1f);

        Debug.Log($"[Boss] ¡Carga!");
        agent.isStopped = false;
        float prevSpeed = agent.speed;
        agent.speed = chargeSpeed;
        agent.SetDestination(player.position);

        float elapsed = 0f;
        while (elapsed < 1.2f && !isDead)
        {
            elapsed += Time.deltaTime;
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= meleeRange)
            {
                playerStats?.TakeDamage(stats.damage * 2.5f);
                FloatingDamageSpawner.Spawn(player.position, stats.damage * 2.5f, DamageType.Physical);
                break;
            }
            yield return null;
        }

        agent.speed = prevSpeed;
    }

    void AoEAttack()
    {
        aoeTimer = aoeAttackCooldown * (currentPhase >= 3 ? 0.65f : 1f);

        float dmg = stats.damage * aoeDamageMultiplier;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= aoeRadius)
        {
            playerStats?.TakeDamage(dmg);
            FloatingDamageSpawner.Spawn(player.position, dmg, DamageType.Physical);
        }
        Debug.Log($"[Boss] AoE attack! Fase {currentPhase}");
    }

    void CheckPhaseTransition(float current, float max)
    {
        float ratio = current / max;

        if (currentPhase == 1 && ratio <= phase2Threshold)
        {
            currentPhase = 2;
            OnPhase2();
        }
        else if (currentPhase == 2 && ratio <= phase3Threshold)
        {
            currentPhase = 3;
            OnPhase3();
        }
    }

    void OnPhase2()
    {
        stats.moveSpeed  *= 1.3f;
        stats.attackSpeed *= 1.25f;
        agent.speed = stats.moveSpeed;
        Debug.Log($"[Boss] FASE 2 — {phase2Message}");
    }

    void OnPhase3()
    {
        stats.moveSpeed  *= 1.4f;
        stats.attackSpeed *= 1.5f;
        stats.damage     *= 1.6f;
        agent.speed = stats.moveSpeed;
        Debug.Log($"[Boss] FASE 3 — {phase3Message}");
    }

    void HandleDeath()
    {
        isDead          = true;
        agent.isStopped = true;

        playerExp?.GainExp(stats.expReward);

        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        Destroy(gameObject, 3f);
    }

    void FaceTarget(Transform t)
    {
        Vector3 dir = (t.position - transform.position).normalized;
        dir.y = 0f;
        if (dir == Vector3.zero) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
    }

    public void TakeDamage(float amount) => stats?.TakeDamage(amount);
}
