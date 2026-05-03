using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyController : MonoBehaviour
{
    [Header("AI")]
    public float detectionRange = 8f;
    public float attackRange    = 1.8f;

    private NavMeshAgent     agent;
    private EnemyStats       stats;
    private Animator         animator;
    private Transform        player;
    private PlayerStats      playerStats;
    private ExperienceSystem playerExp;

    private float attackTimer;
    private bool  isDead;

    public bool IsDead => isDead;

    void Awake()
    {
        agent    = GetComponent<NavMeshAgent>();
        stats    = GetComponent<EnemyStats>();
        animator = GetComponentInChildren<Animator>();
        stats.OnDeath += HandleDeath;
    }

    void Start()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
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

        if (dist <= attackRange)
        {
            agent.isStopped = true;
            animator?.SetFloat("Speed", 0f);
            FaceTarget(player);
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                attackTimer = 1f / stats.attackSpeed;
                AttackPlayer();
            }
        }
        else if (dist <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator?.SetFloat("Speed", agent.velocity.magnitude);
        }
        else
        {
            agent.isStopped = true;
            animator?.SetFloat("Speed", 0f);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        stats.TakeDamage(amount);
    }

    void AttackPlayer()
    {
        animator?.SetTrigger("Attack");
        playerStats?.TakeDamage(stats.damage);
    }

    void FaceTarget(Transform t)
    {
        Vector3 dir = (t.position - transform.position).normalized;
        dir.y = 0f;
        if (dir == Vector3.zero) return;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            8f * Time.deltaTime
        );
    }

    void HandleDeath()
    {
        isDead          = true;
        agent.isStopped = true;
        animator?.SetTrigger("Die");

        // Dar EXP al player
        playerExp?.GainExp(stats.expReward);

        // Desactivar colisión para que no bloquee
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        // TODO: animación de muerte
        Destroy(gameObject, 2f);
    }
}
