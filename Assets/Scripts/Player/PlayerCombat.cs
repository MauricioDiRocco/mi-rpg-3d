using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 2f;
    public float aoeRadius   = 1.5f;   // radio extra para golpear enemigos cercanos al target
    public LayerMask enemyLayer;

    private PlayerStats      stats;
    private PlayerController controller;
    private Animator         animator;

    private EnemyController lockedTarget;
    private float           attackTimer;

    public EnemyController LockedTarget => lockedTarget;

    void Awake()
    {
        stats      = GetComponent<PlayerStats>();
        controller = GetComponent<PlayerController>();
        stats.OnDeath += () => { if (animator) animator.SetTrigger("Die"); };
        stats.OnHit   += () => { if (animator) animator.SetTrigger("GetHit"); };
    }

    void Start()
    {
        // Lazy: el modelo visual puede no estar en Awake
        animator = GetComponentInChildren<Animator>(true);
    }

    void Update()
    {
        if (lockedTarget == null) return;

        if (lockedTarget.IsDead)
        {
            ClearTarget();
            return;
        }

        float dist = Vector3.Distance(transform.position, lockedTarget.transform.position);

        if (dist > attackRange)
        {
            // Perseguir target
            controller.MoveTo(lockedTarget.transform.position);
        }
        else
        {
            // Atacar
            controller.StopMoving();
            FaceTarget(lockedTarget.transform);

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                attackTimer = 1f / stats.AttackSpeed;
                PerformAttack();
            }
        }
    }

    void PerformAttack()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>(true);
        animator?.SetTrigger("Attack");

        // Golpe principal al target bloqueado
        DealDamage(lockedTarget);

        // AoE: golpear enemigos dentro del radio de ataque + aoeRadius
        Collider[] nearby = Physics.OverlapSphere(
            lockedTarget.transform.position,
            aoeRadius,
            enemyLayer
        );
        foreach (var col in nearby)
        {
            EnemyController nearby_enemy = col.GetComponentInParent<EnemyController>();
            if (nearby_enemy != null && nearby_enemy != lockedTarget && !nearby_enemy.IsDead)
                DealDamage(nearby_enemy);
        }
    }

    void DealDamage(EnemyController enemy)
    {
        enemy.TakeDamage(stats.PhysDamage);
    }

    public void SetTarget(EnemyController target)
    {
        lockedTarget = target;
        attackTimer  = 0.3f; // pequeño delay antes del primer golpe
    }

    public void ClearTarget()
    {
        lockedTarget = null;
    }

    void FaceTarget(Transform t)
    {
        Vector3 dir = (t.position - transform.position).normalized;
        dir.y = 0f;
        if (dir == Vector3.zero) return;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            10f * Time.deltaTime
        );
    }
}
