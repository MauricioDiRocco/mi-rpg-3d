using UnityEngine;

// Colocar en el GameObject del Player.
// Detecta IInteractable dentro de interactRange y dispara Interact() al presionar E.
// Expone HasTarget y CurrentPrompt para que HUDController muestre el aviso en pantalla.
public class InteractionSystem : MonoBehaviour
{
    public float interactRange = 3f;

    IInteractable current;

    public bool   HasTarget      => current != null;
    public string CurrentPrompt  => current?.InteractPrompt ?? "";

    void Update()
    {
        FindClosest();
        if (current != null && Input.GetKeyDown(KeyCode.E))
            current.Interact();
    }

    void FindClosest()
    {
        IInteractable best     = null;
        float         bestDist = interactRange + 0.01f;

        // QueryTriggerInteraction.Collide: detecta triggers (SphereCollider de NPCs)
        var cols = Physics.OverlapSphere(transform.position, interactRange, ~0, QueryTriggerInteraction.Collide);
        foreach (var col in cols)
        {
            var inter = col.GetComponentInParent<IInteractable>();
            if (inter == null) continue;
            float d = Vector3.Distance(transform.position, col.transform.position);
            if (d < bestDist) { bestDist = d; best = inter; }
        }

        if (best != current)
        {
            current?.OnExitRange();
            current = best;
            current?.OnEnterRange();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
