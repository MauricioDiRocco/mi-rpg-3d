using UnityEngine;

// Cofre de tesoro interactuable con tecla E.
// Requiere LootDropper en el mismo GO. Spawnea loot una sola vez y desactiva la colisión.
[RequireComponent(typeof(Collider))]
public class TreasureChest : MonoBehaviour, IInteractable
{
    public string displayName = "Cofre";

    bool opened;

    public string InteractPrompt => opened ? "" : $"Abrir {displayName}";

    public void Interact()
    {
        if (opened) return;
        opened = true;

        var dropper = GetComponent<LootDropper>();
        dropper?.Drop();

        // Desactivar collider para que no bloquee otra vez
        GetComponent<Collider>().enabled = false;

        Debug.Log($"[TreasureChest] {displayName} abierto.");
    }

    public void OnEnterRange() { }
    public void OnExitRange()  { }
}
