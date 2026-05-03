using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerController : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

    private NavMeshAgent      agent;
    private PlayerStats       stats;
    private PlayerCombat      combat;
    private EquipmentManager  equipment;
    private CameraController  cam;
    private Animator          animator;  // lazy-found (modelo se reemplaza post-Awake)
    private bool              animParamsReady;

    void Awake()
    {
        agent     = GetComponent<NavMeshAgent>();
        stats     = GetComponent<PlayerStats>();
        combat    = GetComponent<PlayerCombat>();
        equipment = GetComponent<EquipmentManager>();
    }

    void Start()
    {
        cam = Camera.main?.GetComponent<CameraController>();
    }

    void Update()
    {
        // Lazy-find: el modelo visual se instala después del prefab
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>(true);
            if (animator != null) animParamsReady = HasAnimParam("Speed");
        }

        agent.speed = stats.MoveSpeed;

        if (animator != null && animParamsReady)
        {
            var localVel = transform.InverseTransformDirection(agent.velocity);
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetFloat("MoveX", localVel.x);
            // HasWeapon: por ahora true → usa MeleeAttack (EquipmentManager lo actualiza al equipar)
            animator.SetBool("HasWeapon", true);
        }

        HandleInput();
    }

    bool HasAnimParam(string paramName)
    {
        foreach (var p in animator.parameters)
            if (p.name == paramName) return true;
        return false;
    }

    void HandleInput()
    {
        if (Camera.main == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Click izquierdo DOWN: primero buscar enemigo para atacar
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hitEnemy, 200f, enemyLayer))
            {
                var enemy = hitEnemy.collider.GetComponentInParent<EnemyController>();
                if (enemy != null && !enemy.IsDead)
                {
                    combat.SetTarget(enemy);
                    return;
                }
            }
        }

        // Click izquierdo HOLD: mover si no hay target bloqueado
        if (Input.GetMouseButton(0) && combat.LockedTarget == null)
        {
            if (Physics.Raycast(ray, out RaycastHit hitGround, 200f, groundLayer))
            {
                MoveTo(hitGround.point);
            }
        }

        // Click derecho (hold) sin arrastrar cámara: mover y limpiar target
        if (Input.GetMouseButton(1) && cam != null && !cam.IsDraggingCamera)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer))
            {
                MoveTo(hit.point);
                combat.ClearTarget();
            }
        }
    }

    public void MoveTo(Vector3 worldPos)
    {
        // Intento preciso primero (0.5 m), luego amplio (3 m)
        if (NavMesh.SamplePosition(worldPos, out NavMeshHit navHit, 0.5f, NavMesh.AllAreas) ||
            NavMesh.SamplePosition(worldPos, out navHit, 3f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(navHit.position);
        }
    }

    public void StopMoving()
    {
        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
}
