using UnityEngine;

// Coloca este script en el Player. Instancia un prefab visual debajo del enemigo seleccionado.
// Si no tenes prefab, crea un cubo achatado o un círculo plano como indicador.
public class TargetIndicator : MonoBehaviour
{
    public GameObject indicatorPrefab; // asignar en el Inspector

    private PlayerCombat      combat;
    private GameObject        activeIndicator;
    private EnemyController   lastTarget;

    void Awake()
    {
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        EnemyController target = combat.LockedTarget;

        if (target == lastTarget) return;

        // Destruir indicador anterior
        if (activeIndicator != null)
            Destroy(activeIndicator);

        // Crear indicador en el nuevo target
        if (target != null && indicatorPrefab != null)
        {
            activeIndicator = Instantiate(indicatorPrefab, target.transform);
            activeIndicator.transform.localPosition = Vector3.zero;
        }

        lastTarget = target;
    }
}
